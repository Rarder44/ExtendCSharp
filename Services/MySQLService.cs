﻿using ExtendCSharp.Attributes;
using ExtendCSharp.Interfaces;
using ExtendCSharp.Wrapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ExtendCSharp.Services
{
    public class MySQLService : IService
    {
        public MySqlConnection c;
        public Exception LastException { get; private set; }

        public MySQLService(String IpServer, String DBname, String User, String Pass)
        {
            Connect(IpServer, DBname, User, Pass);
        }

        public bool Connect(String IpServer, String DBname, String User, String Pass)
        {
            if (c != null)
                c.TryClose();

            c = new MySqlConnection("Server=" + IpServer + ";Database=" + DBname + ";Uid=" + User + ";Pwd=" + Pass + ";");

            try
            {
                c.Open();
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
            return true;
        }
        public bool Close()
        {
            Exception e = c.TryClose();
            if (e == null)
                return true;
            else
            {
                LastException = e;
                return false;
            }
        }

        public bool ExecuteQuery(String Query)
        {
            MySqlDataReader dr = null;
            try
            {
                MySqlCommand comm = new MySqlCommand(Query, c);
                dr = comm.ExecuteReader();
                dr.TryClose();
                return true;
            }
            catch (Exception e)
            {
                dr.TryClose();
                LastException= e;
                return false;
            }
        }
        public MySqlDataReader ExecuteReaderQuery(String Query)
        {
            try
            {
                MySqlCommand comm = new MySqlCommand(Query, c);
                return comm.ExecuteReader();
            }
            catch { return null; }
        }

        /// <summary>
        /// Ritorna una lista di Righe ( ogni riga è un array di object ). I veri tipi dovranno essere passati tramite la lista TipiDati, in ordine. occorrerà infine fare un cast sull'object 
        /// Leggere il README
        /// </summary>
        /// <param name="Query">Query da eseguire</param>
        /// <param name="TipiDati">Lista di Type -> ( per ottenere un tipe = tipeof(int) / tipeof(String) / ecc... )</param>
        /// <returns></returns>
        /*public List<object[]> ExecuteReaderQuery(String Query,List<Type> TipiDati)
        {
            List<object[]> l = new List<object[]>();
            MySqlDataReader r = null;
            try
            {
               
                MySqlCommand comm = new MySqlCommand(Query, c);
                r =comm.ExecuteReader();
                if(r.HasRows)
                {
                    while (r.Read())
                    {
                        object[] o = new object[TipiDati.Count];
                        for(int i=0;i< TipiDati.Count;i++)
                        {
                            o[i] = r.GetFieldValue(i, TipiDati[i]);
                            
                        }

                        l.Add(o);
                    }
                }

                r.TryClose();
            }
            catch(Exception ex)
            {
                
                r.TryClose();
                throw ex;
            }
            return l;
        }*/

        /// <summary>
        /// Ritorna una lista di Righe ( ogni riga è un array di object ). I veri tipi dovranno essere passati tramite la lista TipiDati, in ordine. occorrerà infine fare un cast sull'object 
        /// Leggere il README
        /// </summary>
        /// <param name="Query">Query da eseguire</param>
        /// <param name="TipiDati">Lista di Type -> ( per ottenere un tipe = tipeof(int) / tipeof(String) / ecc... )</param>
        /// <returns></returns>
        public List<T> ExecuteReaderQuery<T>(String Query) where T : new()
        {
            Type tipo = typeof(T);
            List<T> OutList = new List<T>();
            

            MySqlDataReader r = null;
            try
            {

                MySqlCommand comm = new MySqlCommand(Query, c);
                r = comm.ExecuteReader();
                if (r.HasRows)
                {
                    //Se la query ha restituito delle righe


                    //faccio le operazioni per ottenere dalla classe passata i Field
                    //--------- INIZIO ---------
                    Dictionary<String, Property_Field_Info> fieldDictionary = new Dictionary<string, Property_Field_Info>();

                    //Ottengo il nome delle colonne
                    var columns = Enumerable.Range(0, r.FieldCount).Select(r.GetName).ToList();


                    //Recupero tutti i campi PUBBLICI con l'attributo MySQLFieldAttribute
                    FieldInfo[] Campi = tipo.GetFields().Where(prop => Attribute.IsDefined(prop, typeof(MySQLFieldAttribute))).ToArray();
                    //Recupero tutti le proprietà PUBBLICHE con l'attributo MySQLFieldAttribute e che abbiano il metodo SET

                    PropertyInfo[] Propieta = tipo.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MySQLFieldAttribute)) && prop.GetSetMethod()!=null).ToArray();
                    
                    
                    //TO_REMOVE
                    /*#if (NETFX4_5 || NETFX4_6 || NETFX4_6_1)
                                        PropertyInfo[] Propieta = tipo.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MySQLFieldAttribute)) && prop.SetMethod != null).ToArray();

                    #endif
                    #if (NETFX4_0_3 || NETFX3_5)
                                        PropertyInfo[] Propieta = tipo.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MySQLFieldAttribute)) && prop.CanWrite ).ToArray();
                                        PropertyInfo[] Propieta = tipo.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MySQLFieldAttribute)) && prop.SetMethod != null).ToArray();
                    #endif*/



                    //Cerco in base ai nomi, i Field associati nella classe
                    //e li inserisco nel dictionary (per una ricerca più rapida)
                    foreach ( String s in columns)
                    {
                        
                        FieldInfo tmpFi = Campi.FirstOrDefault(field => Attribute.GetCustomAttribute(field, typeof(MySQLFieldAttribute))._Cast<MySQLFieldAttribute>().Name == s);
                        if(tmpFi != null)
                            fieldDictionary.Add(s, tmpFi);

                        PropertyInfo tmpPi = Propieta.FirstOrDefault(field => Attribute.GetCustomAttribute(field, typeof(MySQLFieldAttribute))._Cast<MySQLFieldAttribute>().Name == s);
                        if (tmpPi != null)
                            fieldDictionary.Add(s, tmpPi);
                    }

                    //--------- FINE ---------

                    
                    
                    //Leggo una riga
                    while (r.Read())
                    {
                        T tmpObj = new T();
                        foreach ( KeyValuePair<string, Property_Field_Info> kv in fieldDictionary )
                        {
                            try
                            {
                                object value = r[kv.Key];
                                Type t= value.GetType();
                                
                                kv.Value.SetValue(tmpObj, value);
                                
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        OutList.Add(tmpObj);
                    }
                }

                r.TryClose();
            }
            catch (Exception ex)
            {

                r.TryClose();
                throw ex;
            }
            return OutList;
        }


        /// <summary>
        ///  Ritorna una lista di Righe ( ogni riga è un dizionario che ha come chiave il nome della colonna e come valore un object ). I veri tipi dovranno essere passati tramite il dizionario TipiDati ( nome colonna, tipo di dato ) , in ordine. occorrerà infine fare un cast sull'object 
        /// leggere il README
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="TipiDati"></param>
        /// <returns></returns>
        /*public List<Dictionary<String, object>> ExecuteReaderQuery(String Query, Dictionary<String,Type> TipiDati)
        {
            List<Dictionary<String, object>> l = new List<Dictionary<String, object>>();

            MySqlDataReader r = null;
            try
            {

                MySqlCommand comm = new MySqlCommand(Query, c);
                r = comm.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Dictionary<String, object> o = new Dictionary<String, object>();
                        foreach (KeyValuePair<String,Type> v in TipiDati)
                        {
                            if (v.Value == typeof(int))
                                o.Add(v.Key, r.GetInt32(v.Key));
                            else if (v.Value == typeof(int) || v.Value == typeof(Int32))
                                o.Add(v.Key, r.GetInt32(v.Key));
                            else if (v.Value == typeof(Int64))
                                o.Add(v.Key, r.GetInt64(v.Key));
                            else if (v.Value == typeof(Int16))
                                o.Add(v.Key, r.GetInt16(v.Key));
                            else if (v.Value == typeof(double))
                                o.Add(v.Key, r.GetDouble(v.Key));
                            else if (v.Value == typeof(float))
                                o.Add(v.Key, r.GetFloat(v.Key));
                            else if (v.Value == typeof(bool))
                                o.Add(v.Key, r.GetBoolean(v.Key));
                            else if (v.Value == typeof(byte))
                                o.Add(v.Key, r.GetByte(v.Key));
                            else if (v.Value == typeof(char))
                                o.Add(v.Key, r.GetChar(v.Key));
                            else if (v.Value == typeof(DateTime))
                                o.Add(v.Key, r.GetDateTime(v.Key));
                            else if (v.Value == typeof(String) || v.Value == typeof(string))
                                o.Add(v.Key, r.GetString(v.Key));
                            else if (v.Value == typeof(TimeSpan))
                                o.Add(v.Key, r.GetTimeSpan(v.Key));
                            else if (v.Value == typeof(TimeSpanPlus))
                                o.Add(v.Key, new TimeSpanPlus(r.GetTimeSpan(v.Key)));
                        }
                        l.Add(o);
                    }
                }

                r.TryClose();
            }
            catch (Exception ex)
            {

                r.TryClose();
                throw ex;
            }
            return l;
        }
        */



        /// <summary>
        /// Permette di eseguire una query e di visualizzare i risultati in un DataGridView
        /// </summary>
        /// <param name="query">Query da eseguire</param>
        /// <param name="dataGridView">DataGridView da riempire</param>
        /// <returns>TRUE = OK | FALSE = c'è stata un eccezione ( visualizzabile nella Propietà LastException ) </returns>
        public bool FillDataGrid(String query, DataGridView dataGridView)
        {
            try
            {
                MySqlDataAdapter adr = new MySqlDataAdapter(query, c);
                adr.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adr.Fill(dt);
                dataGridView.DataSource = dt;
            }
            catch (Exception ex)
            {
                LastException= ex;
                return false;
            }
            return true;
        }
    }

  


}
