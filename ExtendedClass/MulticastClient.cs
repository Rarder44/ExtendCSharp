﻿using ExtendCSharp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ExtendCSharp.ExtendedClass
{

    [Serializable]
    public class MulticastPacket
    {
       
        public static int MaxDatagramLenght { get; private set; } = 1024*60; //32k
        public static int SerializedLenght
        {
            get
            {
                if (serializedLenght == -1)
                    serializedLenght = CalculateSerializedLenght();
                return serializedLenght;
            }
        }
        private static int serializedLenght=-1;
        private static int CalculateSerializedLenght()
        {
            MulticastPacket mp = new MulticastPacket();
            mp.Data = new byte[MaxDatagramLenght];
            return mp.Serialize().Length;
        }


        public ulong GroupNumber { get; set; }
        public int index { get; private set; }
        public bool Last { get; private set; } = false;
        public byte[] Data { get; private set; }

        public static MulticastPacket[] CreatePackets(byte[] Data,ulong GroupNumber)
        {
            byte[][] chunks = Data.Chunkize(MaxDatagramLenght);
            MulticastPacket[] packets = new MulticastPacket[chunks.Length];
            for (int i = 0; i < chunks.Length; i++)
            {
                packets[i] = new MulticastPacket();
                packets[i].Data = chunks[i];
                packets[i].index = i;
                packets[i].GroupNumber = GroupNumber;
            }
            packets.Last().Last = true;
            return packets;
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 
                formatter.Serialize(ms, this); // the serialization process 
                byte[] tmp = ms.ToArray();
                return tmp;
            }
        }
        public static MulticastPacket Deserialize(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(data, 0, data.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    BinaryFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 
                    MulticastPacket m = (MulticastPacket)formatter.Deserialize(ms);
                    return m;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    
    public class MulticastPacketGroup
    {

        //TODO: implemento i gruppi suddivisi per connessione
        Dictionary<ulong, ListPlus<MulticastPacket>> list = new Dictionary<ulong, ListPlus<MulticastPacket>>();

        //ulong? CurrentGroup = null;
        public void AddPacket(MulticastPacket mp)
        {
            if (!list.ContainsKey(mp.GroupNumber))
                list[mp.GroupNumber] = new ListPlus<MulticastPacket>();

            list[mp.GroupNumber][mp.index] = mp;
        }

        public ulong? LastCompleted()
        {
            ulong? LastCompletedIndex = null;
            IOrderedEnumerable<ulong> keys= list.Keys.OrderBy((key)=> { return key; });
            foreach (ulong k in keys)
            {
                if (Completed(list[k]))
                {
                    LastCompletedIndex = k;
                }
            }
            return LastCompletedIndex;
        }

        private bool Completed(ListPlus<MulticastPacket> listToCheck)
        {
            bool isFull = true;
            bool isEnd = false;

            foreach(MulticastPacket p in listToCheck)
            {
                if (p == null)
                {
                    return false;
                }
                else if (p.Last == true)
                    isEnd = true;
            }

            if (isFull && isEnd)
                return true;
            return false;
        }

        public void ClearBeforeIndex(ulong index)
        {
            IOrderedEnumerable<ulong> keys = list.Keys.Where((key)=> { 
                return key <= index ? true : false; 
            }).OrderBy((key) => { return key; });
            foreach (ulong k in keys)
                list.Remove(k);
            
        }
        public void Clear()
        {
            list.Clear();
        }
        public byte[] GetDataByIndex(ulong index)    
        {
            ListPlus<MulticastPacket> list = this.list[index];
            int totalLen = 0;
            for(int i=0;i<list.Count;i++)
            {
                totalLen += list[i].Data.Length;
            }
            byte[] data = new byte[totalLen];
            long ByteWritten = 0;
            for (int i = 0; i < list.Count; i++)
            {
                Array.Copy(list[i].Data, 0, data,ByteWritten, list[i].Data.Length);
                ByteWritten += list[i].Data.Length;
            }

            return data;
        }
    }

    public class MulticastClient:IDisposable
    {
        IPAddress interfaceIPAddress;
        IPAddress MulticastAddress;
        int Listening_SourcePort;

        int DestinationPort;
        
        public Socket Socket;
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MulticastAddress"></param>
        /// <param name="Listening_SourcePort">0 = random</param>
        /// <param name="DestinationPort"></param>
        /// <param name="interfaceIPAddress"></param>
        /// <param name="initializeNow"></param>
        public MulticastClient(string MulticastAddress, int Listening_SourcePort,int DestinationPort, string interfaceIPAddress, bool initializeNow = true)
        {
            this.MulticastAddress = IPAddress.Parse(MulticastAddress);
            this.Listening_SourcePort = Listening_SourcePort;
            this.DestinationPort = DestinationPort;
            this.interfaceIPAddress = IPAddress.Parse(interfaceIPAddress);
            if (initializeNow)
            {
                JoinMulticast();
            }
        }
        

        /// <summary>
        /// Si unisce alla rete multicast
        /// </summary>
        public void JoinMulticast()
        {
            try
            {
                // Create a multicast socket.
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


                // Create an IPEndPoint object. 
                int TmpPort = Listening_SourcePort;
                IPEndPoint IPlocal = new IPEndPoint(interfaceIPAddress, TmpPort);      

                // Bind this endpoint to the multicast socket.
                Socket.Bind(IPlocal);

                // Define a MulticastOption object specifying the multicast group 
                // address and the local IP address.
                // The multicast group address is the same as the address used by the listener.
                MulticastOption mcastOption;
                mcastOption = new MulticastOption(MulticastAddress, interfaceIPAddress);

                Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mcastOption);

            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString());
            }
        }

        public void Close()
        {
            StopListener();
            Socket.Close();
        }

        /// <summary>
        /// Invia un messaggio via connessione multicast
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            IPEndPoint endPoint;

            try
            {
                //Send multicast packets to the listener.
                endPoint = new IPEndPoint(MulticastAddress, DestinationPort);
                Socket.SendTo(ASCIIEncoding.ASCII.GetBytes(message), endPoint);


            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString());
            }

            //Socket.Close();
        }


        ulong GroupNumber = 0;

        /// <summary>
        /// Invia un messaggio via connessione multicast
        /// </summary>
        /// <param name="data"></param>
        public void SendGroup(byte[] data,ulong? GroupNumber=null)
        {
            
            IPEndPoint endPoint;

            try
            {
                //Send multicast packets to the listener.
                endPoint = new IPEndPoint(MulticastAddress, DestinationPort);
                if( GroupNumber==null)        
                    GroupNumber = this.GroupNumber++;
                
                
                MulticastPacket[] packets= MulticastPacket.CreatePackets(data, GroupNumber.Value);
                for (int i = 0; i < packets.Length; i++)
                {
                    SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                    e.RemoteEndPoint = endPoint;
                    byte[] d = packets[i].Serialize();
                    e.SetBuffer(d, 0, d.Length);

                    Socket.SendToAsync(e);
                    e.Completed += (object sender, SocketAsyncEventArgs ee) =>
                    {
                        ee.Dispose();
                    };
                }
                
                

            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString());
            }

            //Socket.Close();
        }

      

        public bool ListenerStatus { get; set; } = false;

        /// <summary>
        /// Permette di far partire il listener ( Task )che ascolta se ci sono byte in arrivo e lancia un evento all'arrivo di un pacchetto
        /// </summary>
        public void StartListen()
        {
            ListenerStatus = true;

            new Task(() =>
            {
                try
                {
                    byte[] bytes = new Byte[MulticastPacket.SerializedLenght];
                    IPEndPoint groupEP = new IPEndPoint(MulticastAddress, Listening_SourcePort);
                    EndPoint remoteEP = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

                    MulticastPacketGroup mpr = new MulticastPacketGroup();
                   
                    while (ListenerStatus)
                    {     
                        int ByteRead=Socket.ReceiveFrom(bytes, ref remoteEP);
                        MulticastPacket mp=MulticastPacket.Deserialize(bytes);
                        mpr.AddPacket(mp);
                        ulong? LastCompletedIndex = mpr.LastCompleted();
                        if( LastCompletedIndex!=null)
                        {
                            onReceivedByte?.Invoke(mpr.GetDataByIndex(LastCompletedIndex.Value), remoteEP);
                            mpr.ClearBeforeIndex(LastCompletedIndex.Value);
                        }
                        
                        
                    }
                }
                catch(Exception ex)
                {

                }

            }).Start();
        }
        public void StopListener()
        {
            ListenerStatus = false;
        }

        public void Dispose()
        {
            Socket.Dispose();       //TODO: ci mette troppo a chiudersi... forse sta ancora aspettando di inviare i dati? 
            StopListener();
        }

        public delegate void ReceivedByteDelegate(byte[] data, EndPoint remoteEP);
        public event ReceivedByteDelegate onReceivedByte;


        
    }
}
