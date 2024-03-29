﻿using ExtendCSharp.Controls;
using ExtendCSharp.ExtendedClass;
using ExtendCSharp.Interfaces;
using ExtendCSharp.Services;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Xml.Serialization;

#if (NETFX3_5)
    using ExtendCSharp._v3_5_Fix;
#endif


namespace ExtendCSharp
{
    public static class Extension
    {

        #region WPF

        
        public static void SetContentInvoke(this System.Windows.Controls.ContentControl c, String Content)
        {
            if (!c.Dispatcher.CheckAccess())
                c.Dispatcher.Invoke(()=>{ c.SetContentInvoke(Content); });
            else
            {
                c.Content = Content;
            }
        }

        public static void IsEnabledInvoke(this System.Windows.UIElement c, bool enabled)
        {
            if (!c.Dispatcher.CheckAccess())
                c.Dispatcher.Invoke(() => { c.IsEnabledInvoke(enabled); });
            else
            {
                c.IsEnabled = enabled;
            }
        }


        public static void ClearInvoke(this System.Windows.Controls.Panel p)
        {
            if (!p.Dispatcher.CheckAccess())
                p.Dispatcher.Invoke(() => { p.ClearInvoke(); });
            else
            {
                p.Children.Clear();
            }
        }


        #endregion
        #region NUMBER
        /// <summary>
        /// Permette di capire se il double passato ha solo parte intera e non decimale
        /// </summary>
        /// <param name="d">Il numero double da controllare</param>
        /// <returns>true = solo intero | false = con parte decimale</returns>
        public static bool IsInteger(this double d)
        {
            return (d - (int)d) == 0;
        }
        public static double Sqrt(this double d)
        {
            return Math.Sqrt(d);
        }
        public static double Sqrt(this int d)
        {
            return Math.Sqrt(d);
        }
        public static int Floor(this double d)
        {
            return Math.Floor(d)._Cast<int>();
        }
        public static int Ceiling(this double d)
        {
            return ((int)Math.Ceiling(d));
        }

        public static double Round(this double value, int digit)
        {
            return Math.Round(value, digit);
        }
        public static double Truncate(this double value)
        {
            return Math.Truncate(value);
        }
        public static float Round(this float value, int digit)
        {
            return (float)Math.Round(value, digit);
        }
        public static float Truncate(this float value)
        {
            return (float)Math.Truncate(value);
        }


        public static float Proportion(this float value, float in_min, float in_max, float out_min, float out_max)
        {
            return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
        public static float Proportion(this byte value, float in_min, float in_max, float out_min, float out_max)
        {
            return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }


        public static string ToHexString(this int Number, bool upperCase = true)
        {
            return Number.ToString(upperCase ? "X2" : "x2");
        }
        public static string ToHexString(this byte Number, bool upperCase = true)
        {
            return Number.ToString(upperCase ? "X2" : "x2");
        }

        public static byte[] ToByteArray(this int n)
        {
            return BitConverter.GetBytes(n);
        }
        public static byte[] ToByteArray(this long n)
        {
            return BitConverter.GetBytes(n);
        }
        public static byte[] ToByteArray(this double n)
        {
            return BitConverter.GetBytes(n);
        }
        public static byte[] ToByteArray(this float n)
        {
            return BitConverter.GetBytes(n);
        }
        public static byte[] ToByteArrayASCII(this String n)
        {
            return Encoding.ASCII.GetBytes(n);
        }


        #endregion

        #region String
        public static bool IsInt(this String d)
        {
            int n;
            return int.TryParse(d, out n);
        }
        public static int ParseInt(this String d)
        {
            return int.Parse(d);
        }
        public static long ParseLong(this String d)
        {
            return long.Parse(d);
        }
        public static bool IsFloat(this String d)
        {
            float n;
            return float.TryParse(d, out n);
        }
        public static float ParseFloat(this String d)
        {

            return float.Parse(d.Replace('.', ','));
        }
        public static bool IsDouble(this String d)
        {
            double n;
            return double.TryParse(d, out n);
        }
        public static double ParseDouble(this String d)
        {
            return double.Parse(d);
        }

        public static List<int> AllIndexesOf(this String str, string value)
        {
            List<int> indexes = new List<int>();
            if (String.IsNullOrEmpty(value))
                return indexes;
            for (int index = 0; ; index += value.Length)
            {
                if (index >= str.Length)
                    return indexes;
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static String Concatenate(this String s, String Separatore, params String[] str)
        {
            String temp = s;
            foreach (String ss in str)
            {
                if (temp != "")
                    temp += Separatore;

                temp += ss;
            }
            return temp;
        }
        public static String Concat(this String s, params String[] str)
        {
            return s.Concatenate("", str);
        }

        public static String[] Split(this String s, params String[] str)
        {
            return s.Split(str, StringSplitOptions.None);
        }
        /// <summary>
        /// Splitta la stringa in 2 sottostringhe in base alla prima occorrenza trovata
        /// </summary>
        /// <param name="s"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Tuple<String, String> SplitFirst(this String s, String str)
        {
            int index = s.IndexOf(str);
            Tuple<String, String> t = null;
            if (index != -1)
            {
                String s1 = s.Substring(0, index);
                String s2 = s.Substring(index + str.Length);   //TODO: controllare che se str è alla fine di s, viene creata una stringa vuota e può dare errore
                t = new Tuple<String, String>(s1, s2);

            }
            return t;
        }

        public static String SplitAndGet(this String s, int Index, params char[] chars)
        {
            String[] t = s.Split(chars);
            if (Index >= t.Length)
                return null;
            return t[Index];
        }
        public static String SplitAndGet(this String s, int Index, params String[] str)
        {
            String[] t = s.Split(str, StringSplitOptions.None);
            if (Index >= t.Length)
                return null;
            return t[Index];
        }

        public static String SplitAndGetLast(this String s, params char[] chars)
        {
            String[] t = s.Split(chars);
            return t.Last();
        }
        public static String SplitAndGetLast(this String s, params String[] str)
        {
            String[] t = s.Split(str, StringSplitOptions.None);
            return t.Last();
        }

        public static String SplitAndGetFirst(this String s, params char[] chars)
        {
            String[] t = s.Split(chars);
            return t.First();
        }
        public static String SplitAndGetFirst(this String s, params String[] str)
        {
            String[] t = s.Split(str, StringSplitOptions.None);
            return t.First();
        }


        public static String RemoveLeft(this String s, params String[] str)
        {
            bool repeat = false;
            String st = s;
            do
            {
                repeat = false;
                foreach (String ss in str)
                    if (st.StartsWith(ss))
                    {
                        st = st.Remove(0, ss.Length);
                        repeat = true;
                    }
            }
            while (repeat == true && str.Length > 1);
            return st;
        }
        public static String RemoveRight(this String s, params String[] str)
        {
            bool repeat = false;
            String st = s;
            do
            {
                repeat = false;
                foreach (String ss in str)
                    if (st.EndsWith(ss))
                    {
                        st = st.Remove(st.LastIndexOf(ss));
                        repeat = true;
                    }
            }
            while (repeat == true && s.Length > 1);
            return st;
        }

        public static String RemoveLeftCaseInsensitive(this String s, params String[] str)
        {
            String origin = s;
            s = RemoveLeft(s.ToUpperInvariant(), str.ToUpperInvariant());
            if (origin.Length != s.Length)
                origin = origin.Substring(origin.Length - s.Length);

            return origin;
        }
        public static String RemoveRightCaseInsensitive(this String s, params String[] str)
        {
            String origin = s;
            s = RemoveRight(s.ToUpperInvariant(), str.ToUpperInvariant());
            if (origin.Length != s.Length)
                origin = origin.Substring(0, s.Length);


            return origin;
        }


        public static String Substring(this String s, string delimiter1, string delimiter2)
        {
            int iD1 = s.IndexOf(delimiter1);
            int iD2 = s.IndexOf(delimiter2);

            if (iD1 == -1)
                iD1 = 0;
            else
                iD1 += delimiter1.Length;

            if (iD2 == -1)
                return s.Substring(iD1);
            else
                return s.Substring(iD1, iD2 - iD1);
        }


        /// <summary>
        /// permette di andare a rimuovere tutte le ripetizioni di una determinata sequeza di carattere alla fine di una stringa e di aggiungerla una volta sola
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static String OneStringEnd(this String s, String c)
        {
            String t = s;
            while (t.EndsWith(c))
                t = t.RemoveRight(c);

            return t + c;
        }
        public static String OneCharEnd(this String s, char c)
        {
            s.TrimEnd(c);
            return s + c;
        }


        public static String OneCharStart(this String s, char c)
        {
            s.TrimStart(c);
            return c + s;
        }

        public static bool RegexIsMatch(this String s, String pattern, RegexOptions o)
        {
            Regex r = new Regex(pattern, o);
            return r.IsMatch(s);
        }
        public static Match RegexMatch(this String s, String pattern, RegexOptions o)
        {
            Regex r = new Regex(pattern, o);
            return r.Match(s);
        }


        /// <summary>
        /// Ripete la replace fin quando non vengono rimosse tutte le occorrenze
        /// </summary>
        /// <param name="s"></param>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        /// <returns></returns>
        public static String ReplaceAsLongExistOccurency(this String s, String OldValue, String NewValue)
        {
            bool finito = false;
            do
            {
                string NewString = s.Replace(OldValue, NewValue);
                if (NewString == s)
                    finito = true;

                s = NewString;
            }
            while (!finito);

            return s;
        }


        public static String ReverseString(this String s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }


        public static String EscapeDoubleQuote(this String s)
        {
            return s.Replace("\"", "\\\"");

        }

        static char[] toEscape = "\0\x1\x2\x3\x4\x5\x6\a\b\t\n\v\f\r\xe\xf\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1a\x1b\x1c\x1d\x1e\x1f\"\\".ToCharArray();
        static string[] literals = @"\0,\x0001,\x0002,\x0003,\x0004,\x0005,\x0006,\a,\b,\t,\n,\v,\f,\r,\x000e,\x000f,\x0010,\x0011,\x0012,\x0013,\x0014,\x0015,\x0016,\x0017,\x0018,\x0019,\x001a,\x001b,\x001c,\x001d,\x001e,\x001f".Split(new char[] { ',' });

        public static String Escape(this String input)
        {
            int i = input.IndexOfAny(toEscape);
            if (i < 0) return input;

            var sb = new System.Text.StringBuilder(input.Length + 5);
            int j = 0;
            do
            {
                sb.Append(input, j, i - j);
                var c = input[i];
                if (c < 0x20) sb.Append(literals[c]); else sb.Append(@"\").Append(c);
            } while ((i = input.IndexOfAny(toEscape, j = ++i)) > 0);

            return sb.Append(input, j, input.Length - j).ToString();
        }
        #endregion

        #region char
        public static bool IsNumber(this char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsUpperChar(this char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        #endregion

        #region String[]

        public static bool Contains(this String[] arr, String str)
        {
            return arr.AsQueryable().Contains<string>(str);
        }

        public static String[] ToUpper(this String[] arr)
        {
            return arr.AsQueryable().Select(s => s.ToUpper()).ToArray();
        }
        public static String[] ToUpperInvariant(this String[] arr)
        {
            return arr.AsQueryable().Select(s => s.ToUpperInvariant()).ToArray();
        }
        public static String[] ToLower(this String[] arr)
        {
            return arr.AsQueryable().Select(s => s.ToLower()).ToArray();
        }
        public static String[] ToLowerInvariant(this String[] arr)
        {
            return arr.AsQueryable().Select(s => s.ToLowerInvariant()).ToArray();
        }

        public static string Join(this String[] arr, String separator)
        {
            return string.Join(separator, arr);
        }

        #endregion

        #region byte[]

        public static string ToHexString(this byte[] bytes, bool upperCase = true)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();

        }
        public static String ToASCIIString(this byte[] s)
        {
            return Encoding.ASCII.GetString(s);
        }
        public static Stream ToStream(this byte[] s)
        {
            return new MemoryStream(s);
        }


        public static int IndexOf(this byte[] s, byte[] pattern)
        {

            int indice = -1;
            for (int i = 0; i < s.Length - pattern.Length; i++)
            {
                bool trovato = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (s[i + j] != pattern[j])
                    {
                        trovato = false;
                        break;
                    }
                }
                if (trovato)
                {
                    indice = i;
                    break;
                }
            }
            return indice;
        }


        public static float ToFloat(this byte[] s)
        {
            return System.BitConverter.ToSingle(s, 0);
        }
        public static int ToInt(this byte[] s)
        {
            return System.BitConverter.ToInt32(s, 0);
        }

        public static byte[][] Chunkize(this byte[] data, int ChunkSize)
        {
            //throw new NotImplementedException("TESTATO! mettere tutto tra try e catch perche crasha");
            int NumberOfChunk = data.Length / ChunkSize;
            if (data.Length % ChunkSize != 0)
                NumberOfChunk++;

            byte[][] chunks = new byte[NumberOfChunk][];
            int index = 0;
            for (int i = 0; i < data.Length; i += ChunkSize)
            {
                chunks[index] = new byte[ChunkSize];
                int ByteToCopy = ChunkSize;
                if (data.Length - i < ChunkSize)
                    ByteToCopy = data.Length - i;
                Array.Copy(data, i, chunks[index], 0, ByteToCopy);
                index++;
            }

            // split on groups with each 100 items
            /*byte[][] chunks = data
                                .Select((s, i) => new { Value = s, Index = i })
                                .GroupBy(x => x.Index / ChunkSize)
                                .Select(grp => grp.Select(x => x.Value).ToArray())
                                .ToArray();*/

            return chunks;
        }

        /// <summary>
        /// Ritorna una stringa contenente un array in formato java 
        /// [1,2,3]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJsString(this byte[] data)
        {

            String s = "[";

            for(int i=0;i<data.Length;i++)
            {
                s += data[i];
                if(i!=data.Length-1)
                {
                    s += ",";
                }
            }
            s += "]";
            return s;
        }

        #endregion

        #region PictureBox
        public static void SetImageInvoke(this PictureBox t, Image b)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetImageInvoke(b); });
            else
                t.Image = b;
        }
        #endregion

        #region Control

        public static void SetHWNDChildInvoke(this Control t,HWND hwnd)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetHWNDChildInvoke(hwnd); });

            else
            {
                hwnd.SetWindowParent(t);
            }
        }
        public static void InvalidateInvoke(this Control t)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.InvalidateInvoke(); });

            else
                t.Invalidate();
        }
        public static void Enable(this Control t)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.Enable(); });

            else
                t.Enabled = true;
        }
        public static void Disable(this Control t)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.Disable(); });

            else
                t.Enabled = false;
        }
        public static void ChangeState(this Control t)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.ChangeState(); });

            else
                t.Enabled = !t.Enabled;
        }

        public static void SuspendLayoutInvoke(this Control self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.SuspendLayoutInvoke(); });
            else
                self.SuspendLayout();

        }
        public static void ResumeLayoutInvoke(this Control self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.ResumeLayoutInvoke(); });
            else
                self.ResumeLayout();

        }
        public static void SetTextInvoke(this Control t, string s)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetTextInvoke(s); });

            else
                t.Text = s;
        }
        public static void AppendTextInvoke(this Control t, string s)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.AppendTextInvoke(s); });
            else
                t.Text = t.Text + s;
        }

        public static void SetEnableInvoke(this Control t, bool b)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetEnableInvoke(b); });
            else
                t.Enabled = b;
        }
        public static void SetVisibleInvoke(this Control t, bool b)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetVisibleInvoke(b); });
            else
                t.Visible = b;
        }
        private static Thread GetControlOwnerThread(this Control ctrl)
        {
            if (ctrl.InvokeRequired)
                return (Thread)ctrl.Invoke((Func<Thread>)delegate { return ctrl.GetControlOwnerThread(); });
            else
                return System.Threading.Thread.CurrentThread;
        }


        public static void SetSizeInvoke(this Control t, int Width, int Height)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetSize(Width, Height); });
            else
                t.SetSize(Width, Height);
        }
        public static Control SetSize(this Control t, int Width, int Height)
        {
            t.Size = new System.Drawing.Size(Width, Height);
            return t;
        }

        public static void SetWidthInvoke(this Control t, int Width)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetWidthInvoke(Width); });
            else
                t.Width = Width;
        }

        public static void SetLocationInvoke(this Control t, int X, int Y)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetLocation(X, Y); });
            else
                t.SetLocation(X, Y);
        }
        public static Control SetLocation(this Control t, int X, int Y)
        {
            t.Location = new Point(X, Y);
            return t;
        }


        public static List<Control> GetSubControls(this Control Control, bool TuttiILivelli)
        {
            List<Control> temp = new List<Control>();

            if (TuttiILivelli)
            {
                foreach (Control cont in Control.Controls)
                {
                    temp.AddRange(GetSubControls(cont, true));
                    temp.Add(cont);
                }
                return temp;
            }
            else
            {
                foreach (Control cont in Control.Controls)
                    temp.Add(cont);
                return temp;
            }
        }

        /// <summary>
        /// Permette di rimuovere il control corrente dal proprio parent
        /// </summary>
        /// <param name="self"></param>
        public static void RemoveFromParent(this Control self)
        {
            if (self.Parent != null)
                self.Parent.Controls.Remove(self);
        }
        public static void CenterX_InParent(this Control self)
        {
            if (self.Parent != null)
            {
                int x = self.Parent.Width / 2 - self.Width / 2;
                self.Location = new Point(x, self.Location.Y);
            }
        }
        public static void CenterY_InParent(this Control self)
        {
            if (self.Parent != null)
            {
                int y = self.Parent.Height / 2 - self.Height / 2;
                self.Location = new Point(self.Location.X, y);
            }
        }
        public static void RefreshInvoke(this Control t)
        {
            if (t.InvokeRequired)
                t.BeginInvoke((MethodInvoker)delegate { t.RefreshInvoke(); });

            else
                t.Refresh();
        }

        public static void AddControlInvoke(this Control t, Control ToAdd)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.AddControlInvoke(ToAdd); });

            else
                t.Controls.Add(ToAdd);

        }
        public static void RemoveControlInvoke(this Control t, Control ToRemove)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.RemoveControlInvoke(ToRemove); });

            else
                t.Controls.Remove(ToRemove);
        }
        public static void RemoveControlsInvoke(this Control t, IEnumerable<Control> ToRemove)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.RemoveControlsInvoke(ToRemove); });

            else
            {
                foreach (Control c in ToRemove)
                {
                    t.Controls.Remove(c);
                }
            }
        }
        public static void RemoveControlsInvoke(this Control t, Predicate<Control> Condition)
        {
            var ToRemove = t.Controls.Where((c) => { return Condition(c); });
            t.RemoveControlsInvoke(ToRemove);
        }

        public static void ClearControlsInvoke(this Control t)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.ClearControlsInvoke(); });

            else
            {
                t.Controls.Clear();
            }
        }

      


        #endregion

        #region CheckBox

        public static void SetCheckedInvoke(this CheckBox t, bool Checked)
        {
            if (t.InvokeRequired)
                t.Invoke((MethodInvoker)delegate { t.SetCheckedInvoke(Checked); });

            else
                t.Checked = Checked;
        }

        #endregion
        #region ToolStripItem
        public static void SetTextInvoke(this ToolStripItem t, string s)
        {
            if (t == null || t.GetCurrentParent() == null)
                return;

            if (t.GetCurrentParent().InvokeRequired)
            {
                try
                {
                    t.GetCurrentParent().Invoke((MethodInvoker)delegate { t.SetTextInvoke(s); });
                }catch(Exception ex)
                {

                }
            }



            else
                t.Text = s;
        }

        #endregion

        #region Control.ControlCollection
        public static void AddVertical(this Control.ControlCollection self, Control c)
        {
            int y = 0;
            foreach (Control cc in self)
            {
                if (cc.Location.Y + cc.Height > y)
                    y = cc.Location.Y + cc.Height;
            }
            c.Location = new System.Drawing.Point(0, y);
            self.Add(c);
        }
        public static void Replace(this Control.ControlCollection self, Control ToRemove, Control ToAdd)
        {
            ToAdd.Size = ToRemove.Size;
            ToAdd.Location = ToRemove.Location;
            self.Remove(ToRemove);
            self.Add(ToAdd);
        }
        public static IEnumerable<Control> Where(this Control.ControlCollection self, Func<Control, bool> predicate)
        {
            return self.Cast<Control>().Where(predicate);
        }
        public static IEnumerable<Control> Where(this Control.ControlCollection self, Func<Control, int, bool> predicate)
        {
            return self.Cast<Control>().Where(predicate);
        }



        #endregion

        #region FileInfo
        public static string GetFileNameWithoutExtension(this FileInfo self)
        {
            return Path.GetFileNameWithoutExtension(self.Name);
        }
        #endregion

        #region ProgressBar
        public static void SetValueInvoke(this ProgressBar p, int Value)
        {
            if (p.InvokeRequired)
                p.Invoke((MethodInvoker)delegate { p.SetValueInvoke(Value); });
            else
                if (Value < p.Maximum)
                p.Value = Value;
        }
        public static void SetMaximumInvoke(this ProgressBar p, int Maximum)
        {
            if (p.InvokeRequired)
                p.Invoke((MethodInvoker)delegate { p.SetMaximumInvoke(Maximum); });
            else
                p.Maximum = Maximum;
        }


        public static void SetValueNoAnimation(this ProgressBar p, int value)
        {
            if (value > p.Maximum)
                value = p.Maximum;

            if (value == p.Maximum)
            {
                p.Maximum = value + 1;
                p.Value = value + 1;
                p.Maximum = value;
            }
            else
            {
                p.Value = value + 1;
            }
            p.Value = value;
        }
        public static void SetValueNoAnimationInvoke(this ProgressBar p, int value)
        {
            if (p.InvokeRequired)
                p.Invoke((MethodInvoker)delegate { p.SetValueNoAnimation(value); });
            else
            {
                p.SetValueNoAnimation(value);
            }
        }


        #endregion

        #region ComboBox

        public static void FillWithEnum<T>(this ComboBox comboBox) where T : struct
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T deve essere un Enum", "enumerationValue");
            }

            EnumService es = ServicesManager.GetOrSet(() => { return new EnumService(); });





            comboBox.ValueMember = "Value";     //in Item1 c'è il value
            comboBox.DisplayMember = "Key"; //in Item2 c'è la stringa

            comboBox.DataSource = new BindingSource(es.GetDictionary<T>(), null);

        }


        #endregion

        #region IList

        //ListBox.ObjectCollection

        public static void AddRangeUnique(this IList self, IEnumerable elements)
        {
            foreach (object o in elements)
            {
                self.AddUnique(o);
            }

        }
        public static void AddUnique(this IList self, object obj)
        {
            if (!self.Contains(obj))
                self.Add(obj);
        }


        public static List<T> ToList<T>(this IList self)
        {

            List<T> temp = new List<T>();
            foreach (T o in self)
                temp.Add(o);

            return temp;
        }
        public static ListPlus<T> ToListPlus<T>(this IList self)
        {

            ListPlus<T> temp = new ListPlus<T>();
            foreach (T o in self)
                temp.Add(o);
            return temp;
        }



        public static void SwapInvoke(this IList self, int item1, int item2)
        {

            if (item1 < self.Count && item2 < self.Count)
            {
                object t = self[item1];
                self[item1] = self[item2];
                self[item2] = t;
            }

        }

        public static T RemoveAndGet<T>(this IList<T> self, int index, T DefaultValue = default(T))
        {
            if (index < self.Count)
            {
                DefaultValue = self[index];
                self.RemoveAt(index);
            }

            return DefaultValue;
        }


        #endregion
        #region ListBox
        public static void RemoveSelectedItemsInvoke(this ListBox self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.RemoveSelectedItemsInvoke(); });
            else
            {
                while (self.SelectedItem != null)
                {
                    self.Items.Remove(self.SelectedItems[0]);
                }
            }
        }


        public static void ClearInvoke(this ListBox self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.ClearInvoke(); });
            else
            {
                self.Items.Clear();
            }
        }
        public static void AddInvoke(this ListBox self, object obj)
        {
            try
            {
                if (self.InvokeRequired)
                    self.Invoke((MethodInvoker)delegate { self.AddInvoke(obj); });
                else
                {
                    self.Items.Add(obj);
                }
            }
            catch (Exception) { };
        }


        public static void AddInvoke(this ListBox self, object[] obj)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.AddInvoke(obj); });
            else
            {
                self.Items.AddRange(obj);
            }
        }
        public static void AddInvoke(this ListBox self, ListBox.ObjectCollection obj)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.AddInvoke(obj); });
            else
            {
                self.Items.AddRange(obj);
            }
        }


        public static void RemoveInvoke(this ListBox self, object obj)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.RemoveInvoke(obj); });
            else
            {
                self.Items.Remove(obj);
            }
        }




        public static object GetAtInvoke(this ListBox self, int i)
        {
            if (self.InvokeRequired)
                return self.Invoke((Func<object>)delegate { return self.GetAtInvoke(i); });
            else
            {
                if (i >= self.Items.Count)
                    return null;
                return self.Items[i];
            }

        }
        public static object SelectedItemInvoke(this ListBox self)
        {
            if (self.InvokeRequired)
                return self.Invoke((Func<object>)delegate { return self.SelectedItem; });
            else
            {
                return self.SelectedItem;
            }

        }


        public static int GetItemsCountInvoke(this ListBox self)
        {
            if (self.InvokeRequired)
                return (int)self.Invoke((Func<int>)delegate { return self.GetItemsCountInvoke(); });
            else
            {
                return self.Items.Count;
            }

        }

        public static void RemoveAtInvoke(this ListBox self, int i)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.RemoveAtInvoke(i); });
            else
            {
                if (i < self.Items.Count)
                    self.Items.RemoveAt(i);
            }
        }


        public static void SwapInvoke(this ListBox self, int item1, int item2)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.SwapInvoke(item1, item2); });
            else
            {
                if (item1 < self.Items.Count && item2 < self.Items.Count)
                {
                    object t = self.Items[item1];
                    self.Items[item1] = self.Items[item2];
                    self.Items[item2] = t;
                }

            }
        }


        public static void SetSelectedIndexInvoke(this ListBox self, int index)
        {

            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.SetSelectedIndexInvoke(index); });
            else
            {
                if (index < self.Items.Count && index >= 0)
                    self.SelectedIndex = index;
            }

        }
        public static void ScrollToEnd(this ListBox self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.ScrollToEnd(); });
            else
            {
                self.SelectedIndex = self.Items.Count - 1;
                self.SelectedIndex = -1;
            }
        }

        #endregion

        #region List<String>

        public static void AddToLower(this List<String> self, String s)
        {
            self.Add(s.ToLower());
        }
        public static void AddToUpper(this List<String> self, String s)
        {
            self.Add(s.ToUpper());
        }

        #endregion

        #region List<T>

        public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
        public static void Remove<T>(this IList<T> self, IEnumerable<T> ToRemove)
        {
            foreach (T t in ToRemove)
                self.Remove(t);
        }





        #endregion

        #region Dictionary<>
        public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            Random r = new Random();
            return source.OrderBy(x => r.Next()).ToDictionary(item => item.Key, item => item.Value);
        }
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }

        public static Dictionary<TKey, TValue> ClonePlus<TKey, TValue>(this Dictionary<TKey, TValue> original) where TValue : ICloneablePlus
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }

        public static void Remove<TKey, TValue>(this Dictionary<TKey, TValue> source, TValue element)
        {
            List<TKey> ToRemove = new List<TKey>();
            foreach (KeyValuePair<TKey, TValue> entry in source)
            {
                if (entry.Value.Equals(element))
                    ToRemove.Add(entry.Key);
            }

            foreach (TKey k in ToRemove)
                source.Remove(k);
        }
        public static TValue RemoveAndGet<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue DefaultValue = default(TValue))
        {
            if(self.ContainsKey(key))
            {
                DefaultValue = self[key];
                self.Remove(key);
            }
            return DefaultValue;
        }


        public static IEnumerable<TValue> ToIEnumerable<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            return source.Select(x => { return x.Value; });
        }
        public static bool ContainsKeys<TKey, TValue>(this Dictionary<TKey, TValue> source, params TKey[] keys)
        {
            foreach (TKey s in keys)
            {
                if (!source.ContainsKey(s))
                    return false;
            }
            return true;
        }
        public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> match)
        {
            foreach (var key in dict.Keys.ToArray()
                    .Where(key => match(key, dict[key])))
                dict.Remove(key);
        }


        #endregion

        #region CheckedListBox

        public static void RemoveChecked(this CheckedListBox self)
        {
            object[] indici = self.CheckedItems.ToArray<object>();
            foreach (object o in indici)
            {
                //self.RemoveInvoke(o);
                self.Items.Remove(o);

            }
        }
        public static void RemoveSelected(this CheckedListBox self)
        {
            int i;
            while ((i = self.SelectedIndex) != -1)
            {
                //self.RemoveAtInvoke(i);
                self.Items.RemoveAt(i);
            }
        }

        public static void ClearChecked(this CheckedListBox self)
        {
            foreach (int i in self.CheckedIndices)
            {
                self.SetItemChecked(i, false);
            }
        }

        public static void SwapInvoke(this CheckedListBox self, int item1, int item2)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.SwapInvoke(item1, item2); });
            else
            {
                if (item1 < self.Items.Count && item2 < self.Items.Count)
                {
                    object t = self.Items[item1];
                    bool Check1 = self.GetItemChecked(item1);
                    self.Items[item1] = self.Items[item2];
                    self.SetItemChecked(item1, self.GetItemChecked(item2));
                    self.Items[item2] = t;
                    self.SetItemChecked(item2, Check1);
                }

            }
        }

        #endregion

        #region ICollection

        public static T[] ToArray<T>(this ICollection self)
        {
            return self.OfType<T>().ToArray();
        }
        public static int LastIndex(this ICollection self)
        {
            return self.Count - 1;
        }


        #endregion

        #region DataGridView
        public static void SwapInvoke(this DataGridView self, int c1, int r1, int c2, int r2)
        {
            if (self.InvokeRequired)
            {
                self.Invoke((MethodInvoker)delegate { self.SwapInvoke(c1, r1, c2, r2); });
            }
            else
                if (c1 < self.Columns.Count && c2 < self.Columns.Count && r1 < self.Rows.Count && r2 < self.Rows.Count)
            {
                object o1 = self[c1, r1].Value;
                self[c1, r1].Value = self[c2, r2].Value;
                self[c2, r2].Value = o1;
            }
        }

        public static int GetColumnCountInvoke(this DataGridView self)
        {
            if (self.InvokeRequired)
                return (int)self.Invoke((Func<int>)delegate { return self.GetColumnCountInvoke(); });
            else
                return self.Columns.Count;
        }
        public static int GetRowCountInvoke(this DataGridView self)
        {
            if (self.InvokeRequired)
                return (int)self.Invoke((Func<int>)delegate { return self.GetRowCountInvoke(); });
            else
                return self.Rows.Count;
        }

        public static object GetAtInvoke(this DataGridView self, int c, int r)
        {
            if (self.InvokeRequired)
                return self.Invoke((Func<object>)delegate { return self.GetAtInvoke(c, r); });
            else
                return self[c, r].Value;
        }
        public static void SetAtInvoke(this DataGridView self, int c, int r, object o)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.SetAtInvoke(c, r, o); });
            else
                self[c, r].Value = o;
        }

        public static void SetColumNameInvoke(this DataGridView self, int Column, String Name)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.SetColumNameInvoke(Column, Name); });
            else
            {
                self.Columns[Column].HeaderText = Name;
            }
        }

        public static int GetNotNullElementInColumnInvoke(this DataGridView self, int Column)
        {
            if (self.InvokeRequired)
                return (int)self.Invoke((Func<int>)delegate { return self.GetNotNullElementInColumnInvoke(Column); });
            else
            {
                if (Column < self.Columns.Count)
                {
                    int c = 0;
                    for (int i = 0; i < self.Rows.Count; i++)
                        if (self[Column, i].Value != null)
                            c++;
                    return c;
                }
                return 0;
            }
        }
        public static void RemoveEmptyRowInvoke(this DataGridView self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.RemoveEmptyRowInvoke(); });
            else
            {
                List<DataGridViewRow> lr = new List<DataGridViewRow>();
                for (int i = 0; i < self.Rows.Count; i++)
                {
                    bool allNull = true;
                    for (int j = 0; j < self.Columns.Count; j++)
                    {
                        if (self[j, i].Value != null)
                        {
                            allNull = false;
                        }
                    }
                    if (allNull)
                        lr.Add(self.Rows[i]);
                }
                foreach (DataGridViewRow r in lr)
                {
                    self.Rows.Remove(r);
                }
            }
        }


        public static void ShiftUpInvoke(this DataGridView self, int Column, int RemovePosition)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.ShiftUpInvoke(Column, RemovePosition); });
            else
            {
                for (int i = RemovePosition; i < self.Rows.Count - 1; i++)
                    self[Column, i].Value = self[Column, i + 1].Value;
                self[Column, self.Rows.Count - 1].Value = null;
            }
        }
        public static void ShiftDownInvoke(this DataGridView self, int Column, int Position)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.ShiftDownInvoke(Column, Position); });
            else
            {
                for (int i = self.Rows.Count - 1; i > Position; i--)
                    self[Column, i].Value = self[Column, i - 1].Value;
                self[Column, Position].Value = null;
            }
        }





        #endregion

        #region object
        public static T _Cast<T>(this object o)
        {
            return (T)o;
        }
        #endregion

        #region Point


        public static bool InPoligon(this Point p, Point[] Points)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = Points.Length - 1; i < Points.Length; j = i++)
            {
                if (((Points[i].Y > p.Y) != (Points[j].Y > p.Y)) &&
                 (p.X < (Points[j].X - Points[i].X) * (p.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X))
                    c = !c;
            }
            return c;
        }

        public static Point Rotate(this Point source, Point CentroRotazione, double Gradi)
        {
            return source.Rotate(CentroRotazione.X, CentroRotazione.Y, Gradi);
        }
        public static Point Rotate(this Point source, int CentroRotazioneX, int CentroRotazioneY, double Gradi)
        {
            MathService ms = ServicesManager.GetOrSet(() => { return new MathService(); });
            Gradi = ms.GradToRad(Gradi);
            double px = Math.Cos(Gradi) * (source.X - CentroRotazioneX) - Math.Sin(Gradi) * (source.Y - CentroRotazioneY) + CentroRotazioneX;
            double py = Math.Sin(Gradi) * (source.X - CentroRotazioneX) + Math.Cos(Gradi) * (source.Y - CentroRotazioneY) + CentroRotazioneY;
            return new Point((int)px, (int)py);
        }

        public static PointF Rotate(this PointF source, PointF CentroRotazione, double Gradi)
        {
            return source.Rotate(CentroRotazione.X, CentroRotazione.Y, Gradi);
        }
        public static PointF Rotate(this PointF source, float CentroRotazioneX, float CentroRotazioneY, double Gradi)
        {
            MathService ms = ServicesManager.GetOrSet(() => { return new MathService(); });
            Gradi = ms.GradToRad(Gradi);
            double px = Math.Cos(Gradi) * (source.X - CentroRotazioneX) - Math.Sin(Gradi) * (source.Y - CentroRotazioneY) + CentroRotazioneX;
            double py = Math.Sin(Gradi) * (source.X - CentroRotazioneX) + Math.Cos(Gradi) * (source.Y - CentroRotazioneY) + CentroRotazioneY;
            return new PointF((float)px, (float)py);
        }


        public static double Distanza(this Point source, Point pnt)
        {
            return source.Distanza(pnt.X, pnt.Y);
        }
        public static double Distanza(this Point source, int X, int Y)
        {
            return Math.Sqrt(Math.Pow(source.X - X, 2) + Math.Pow(source.Y - Y, 2));
        }
        public static double Distanza(this PointF source, PointF pnt)
        {
            return source.Distanza(pnt.X, pnt.Y);
        }
        public static double Distanza(this PointF source, float X, float Y)
        {
            return Math.Sqrt(Math.Pow(source.X - X, 2) + Math.Pow(source.Y - Y, 2));
        }
        public static double Distanza(this System.Windows.Point source, System.Windows.Point pnt)
        {
            return source.Distanza(pnt.X, pnt.Y);
        }
        public static double Distanza(this System.Windows.Point source, double X, double Y)
        {
            return Math.Sqrt(Math.Pow(source.X - X, 2) + Math.Pow(source.Y - Y, 2));
        }


        public static double DistanzaAlQuandrato(this Point source, Point pnt)
        {
            return source.DistanzaAlQuandrato(pnt.X, pnt.Y);
        }
        public static double DistanzaAlQuandrato(this Point source, int X, int Y)
        {
            int dy = source.Y - Y;
            int dx = source.X - X;
            return dy * dy - dx * dx;
        }



        public static double Orientamento(this PointF source, PointF pnt)
        {
            return source.Orientamento(pnt.X, pnt.Y);
        }
        public static double Orientamento(this PointF source, float X, float Y)
        {
            MathService ms = ServicesManager.GetOrSet(() => { return new MathService(); });
            float xDiff = X - source.X;
            float yDiff = Y - source.Y;
            return ms.RadToGrad(Math.Atan2(yDiff, xDiff));
        }


        public static Point Add(this Point source, Point pnt)
        {
            return source.Add(pnt.X, pnt.Y);
        }
        public static Point Add(this Point source, float x, float y)
        {
            return source.Add((int)x, (int)y);
        }
        public static Point Add(this Point source, double x, double y)
        {
            return source.Add((int)x, (int)y);
        }
        public static Point Add(this Point source, int x, int y)
        {
            return new Point(source.X + x, source.Y + y);
        }
        public static Point Add(this Point source, Size s)
        {
            return new Point(source.X + s.Width, source.Y + s.Height);
        }

        public static PointF Add(this PointF source, double x, double y)
        {
            return source.Add(x, y);
        }
        public static PointF Add(this PointF source, PointF pnt)
        {
            return source.Add(pnt.X, pnt.Y);
        }
        public static PointF Add(this PointF source, float x, float y)
        {
            return new PointF(source.X + x, source.Y + y);
        }





        public static Point Sub(this Point source, Point pnt)
        {
            return source.Sub(pnt.X, pnt.Y);
        }
        public static Point Sub(this Point source, float x, float y)
        {
            return source.Sub((int)x, (int)y);
        }
        public static Point Sub(this Point source, double x, double y)
        {
            return source.Sub((int)x, (int)y);
        }
        public static Point Sub(this Point source, int x, int y)
        {
            return new Point(source.X - x, source.Y - y);
        }





        public static System.Windows.Point Scala(this System.Windows.Point source, double scala)
        {
            return new System.Windows.Point(source.X * scala, source.Y * scala);
        }
        public static Point Scala(this Point source, double scala)
        {
            return new Point((int)(source.X * scala), (int)(source.Y * scala));
        }
        public static PointF Scala(this PointF source, double scala)
        {
            return new PointF((float)(source.X * scala), (float)(source.Y * scala));
        }

        public static Point Round(this PointF source)
        {
            return Point.Round(source);
        }
        public static Point Truncate(this PointF source)
        {
            return Point.Truncate(source);
        }


        public static Rectangle CreateRectangle(this Point source, int Margin)
        {
            return new Rectangle(source.X - Margin, source.Y - Margin, Margin * 2, Margin * 2);
        }
        public static RectangleF CreateRectangle(this PointF source, float Margin)
        {
            return new RectangleF(source.X - Margin, source.Y - Margin, Margin * 2, Margin * 2);
        }

        public static Point Clone(this Point source)
        {
            return new Point(source.X, source.Y);
        }





        #region Point[]


        public static Point[] Rotate(this Point[] source, Point CentroRotazione, double Gradi)
        {
            return source.Rotate(CentroRotazione.X, CentroRotazione.Y, Gradi);
        }
        public static Point[] Rotate(this Point[] source, int CentroRotazioneX, int CentroRotazioneY, double Gradi)
        {
            Point[] temp = (Point[])source.Clone();
            for (int i = 0; i < temp.Length; i++)
                temp[i] = temp[i].Rotate(CentroRotazioneX, CentroRotazioneY, Gradi);
            return temp;
        }

        public static bool InPoligon(this Point[] Points, Point p)
        {
            return p.InPoligon(Points);
        }


        #endregion




        #endregion

        #region Size

        public static Size Scala(this Size source, double scala)
        {
            return new Size((int)(source.Width * scala), (int)(source.Height * scala));
        }
        public static SizeF Scala(this SizeF source, double scala)
        {
            return new SizeF((float)(source.Width * scala), (float)(source.Height * scala));
        }
        public static Size Add(this Size source, int Width,int Height)
        {
            return new Size(source.Width + Width, source.Height + Height);
        }

        #endregion

        #region Uri
        public static void Append(this Uri self, String s)
        {
            self = new Uri(self, s);
        }
        #endregion

        #region Form



        public static void CloseInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.CloseInvoke(); });
            else
                self.Close();

        }
        
        public static void DisableInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.DisableInvoke(); });
            else
                self.Disable();

        }
        public static void HideInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate {
                    self.HideInvoke();
                });
            else
                self.Hide();

        }
        public static void ShowInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.ShowInvoke(); });
            else
                self.Show();

        }
        public static void ShowDialogInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.ShowDialog(); });
            else
                self.ShowDialog();

        }


        public static void BringToFrontInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.BringToFrontInvoke(); });
            else
                self.BringToFront();

        }
        public static void SetWindowStateInvoke(this Form self, FormWindowState fws)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.WindowState = fws; });
            else
                self.WindowState = fws;

        }
        public static void SetTopMostInvoke(this Form self, bool TopMost)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { self.TopMost = TopMost; });
            else
                self.TopMost = TopMost;

        }

        public static void LockSizeInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { LockSizeInvoke(self); });
            else
            {
                self.MaximumSize = self.Size;
                self.MinimumSize = self.Size;
            }

        }
        public static void UnLockSizeInvoke(this Form self)
        {
            if (self.InvokeRequired)
                self.Invoke((MethodInvoker)delegate { UnLockSizeInvoke(self); });
            else
            {
                self.MaximumSize = new Size(0, 0);
                self.MinimumSize = new Size(0, 0);
            }

        }



        #endregion

        #region Enum
        public static String ToStringSpace(this Enum e)
        {
            return e.ToStringReplace("_", " ");
        }
        public static String ToStringReplace(this Enum e, String find, String replace)
        {
            return e.ToString().Replace(find, replace);
        }
        public static String ToString(this Enum e)
        {
            return e.ToString();
        }

        public static bool IsAn<T>(this Enum e)
        {
            return e is T;
        }

        private static void CheckIsEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
        }
        public static bool IsEnum<T>(this T e, bool withFlags) where T : struct
        {
            if (!typeof(T).IsEnum)
                return false;
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                return false;

            return true;
        }

        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            value.IsEnum(true);
            foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (value.IsFlagSet(flag))
                    yield return flag;
            }
        }
        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            value.IsEnum(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static T SetFlags<T>(this T value, T flags, bool on) where T : struct
        {
            value.IsEnum(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (on)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        public static T SetFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, true);
        }

        public static T ClearFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, false);
        }


        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = 0;
            foreach (T flag in flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }


        /// <summary>
        /// Utilizzo la notazione [Description("Nome da visualizzare")] sulla enum per specificare un testo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue"></param>
        /// <returns></returns>
        public static string ToStringEnum<T>(this T enumerationValue) where T : struct
        {

            enumerationValue.IsEnum(false);

            Type type = enumerationValue.GetType();

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return (attrs[0] as DescriptionAttribute).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }




        #endregion

        #region Graphics

        public static void DrawLines(this Graphics g, Pen pen, params Point[] p)
        {
            g.DrawLines(pen, p);
        }

        public static void DrawPolygon(this Graphics g, Pen pen, params Point[] p)
        {
            g.DrawPolygon(pen, p);
        }
        public static void DrawPolygon(this Graphics g, Pen pen, params PointF[] p)
        {
            g.DrawPolygon(pen, p);
        }


        public static void DrawCircle(this Graphics g, Pen pen, float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius, radius + radius, radius + radius);
        }
        public static void DrawCircle(this Graphics g, Pen pen, PointF Center, float radius)
        {
            g.DrawEllipse(pen, Center.X - radius, Center.Y - radius, radius + radius, radius + radius);
        }

        public static void DrawRectangle(this Graphics g, Pen pen, Point TopLeft, Size size)
        {
            g.DrawRectangle(pen, TopLeft.X, TopLeft.Y, size.Width, size.Height);
        }

        public static void FillCircle(this Graphics g, Brush brush, float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius, radius + radius, radius + radius);
        }

        public static void DrawArc(this Graphics g, Pen pen, float xCenter, float yCenter, float radius, float StartAngle, float SweepAngle)
        {

            g.DrawArc(pen, xCenter - radius, yCenter - radius, radius * 2, radius * 2, StartAngle, SweepAngle);
        }



        /// <summary>
        /// Permette di disegnare una PictureBoxPlus in base alle proprie cordinate/dimensioni
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="pictureBox">PictureBoxPlus da disegnare</param>
        public static void DrawPictureBoxPlus(this Graphics g, PictureBoxPlus pictureBox, bool trasparentBackground = true)
        {
            g.DrawPictureBoxPlus(pictureBox, pictureBox.X, pictureBox.Y, false, trasparentBackground);
        }

        /// <summary>
        /// Permette di disegnare una PictureBoxPlus nelle cordinate fornite
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pictureBox"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Center">Serve per definire se le cordinate si riferiscono al centro della picture box</param>
        public static void DrawPictureBoxPlus(this Graphics g, PictureBoxPlus pictureBox, int x, int y, bool Center = false, bool trasparentBackground = true)
        {
            if (pictureBox == null)
                return;

            if (Center)
            {
                x -= pictureBox.Width / 2;
                y -= pictureBox.Height / 2;
            }
            if (trasparentBackground)
            {
                //pictureBox.DrawOverGraphics(g, x, y);
                g.DrawImage((Image)pictureBox.Foreground, x, y, pictureBox.Foreground.Width, pictureBox.Foreground.Height);
            }
            else
            {
                g.DrawImage((Image)pictureBox.UnitedBitmap, x, y, pictureBox.UnitedBitmap.Width, pictureBox.UnitedBitmap.Height);
            }



        }


        /* public static void DrawImageInvoke(this Graphics g, Image image, int x, int y, int width,int height)
         {
             g.DrawImage(image, x, y, width, height);

         }
         */

        #endregion

        #region Image
        private const int exifOrientationID = 0x112; //274
        public static Image CloneFast(this Image img)
        {
            int Width = img.Width;
            int Height = img.Height;
            Bitmap bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(img, 0, 0, Width, Height);
            }
            return bmp;
        }

        /// <summary>
        /// Ruota un immagine in base al suo valore EXIF ( metadata ) di orientamento
        /// </summary>
        /// <param name="img"></param>
        public static void ExifRotate(this Image img)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
        }

        #endregion

        #region MySQL

        public static Exception TryClose(this MySqlDataReader dr)
        {
            try
            {
                dr.Close();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }
        public static Exception TryClose(this MySqlConnection dr)
        {
            try
            {
                dr.Close();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }


        public static object GetFieldValue(this MySqlDataReader dr, int i, Type tipo)
        {
            try
            {
                MethodInfo method = dr.GetType().GetMethod("GetFieldValue");
                method = method.MakeGenericMethod(tipo);
                return method.Invoke(dr, new object[] { i });
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region TextBox
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);


        public static void Caret(this TextBox dr, bool Visible)
        {
            if (Visible)
                ShowCaret(dr.Handle);
            else
                HideCaret(dr.Handle);
        }
        #endregion

        #region IEnumerable

        /// <summary>
        /// Trasforma un IEnumerable in un ListPlus
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ListPlus<T> ToListPlus<T>(this IEnumerable<T> data)
        {
            return new ListPlus<T>(data);
        }

        /// <summary>
        /// Permette di "appiattire" una struttura ad albero in un IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f)
        {
            return e.SelectMany(c => f(c).Flatten(f)).Concat(e);
        }
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        #endregion

        #region Array

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static T[] Resize<T>(this T[] data, int NewSize)
        {
            Array.Resize(ref data, NewSize);
            return data;
        }
        public static T[] SubArray<T>(this T[] data, int index)
        {
            int length = data.Count() - index;
            if (length == 0)
                return null;

            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        #endregion

        #region Bitmap

        public static Bitmap TrimBitmap(this Bitmap source)
        {
            int x;
            return source.TrimBitmap(out x, out x);
        }

        public static Bitmap TrimBitmap(this Bitmap source, out int LeftCrop, out int TopCrop)
        {
            LeftCrop = 0;
            TopCrop = 0;
            Rectangle srcRect = default(Rectangle);
            BitmapData data = null;
            try
            {
                data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] buffer = new byte[data.Height * data.Stride];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

                int xMin = int.MaxValue,
                    xMax = int.MinValue,
                    yMin = int.MaxValue,
                    yMax = int.MinValue;

                bool foundPixel = false;

                // Find xMin
                for (int x = 0; x < data.Width; x++)
                {
                    bool stop = false;
                    for (int y = 0; y < data.Height; y++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            xMin = x;
                            stop = true;
                            foundPixel = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                // Image is empty...
                if (!foundPixel)
                    return null;

                // Find yMin
                for (int y = 0; y < data.Height; y++)
                {
                    bool stop = false;
                    for (int x = xMin; x < data.Width; x++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            yMin = y;
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                // Find xMax
                for (int x = data.Width - 1; x >= xMin; x--)
                {
                    bool stop = false;
                    for (int y = yMin; y < data.Height; y++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            xMax = x;
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                // Find yMax
                for (int y = data.Height - 1; y >= yMin; y--)
                {
                    bool stop = false;
                    for (int x = xMin; x <= xMax; x++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            yMax = y;
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                        break;
                }

                TopCrop = yMin;
                LeftCrop = xMin;
                srcRect = Rectangle.FromLTRB(xMin, yMin, xMax + 1, yMax + 1);
            }
            finally
            {
                if (data != null)
                    source.UnlockBits(data);
            }

            Bitmap dest = new Bitmap(srcRect.Width, srcRect.Height);
            Rectangle destRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
            using (Graphics graphics = Graphics.FromImage(dest))
            {
                graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return dest;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(IntPtr b1, IntPtr b2, UIntPtr count);

        public static bool EqualMemCmp(this Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, (UIntPtr)len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }


        #endregion

        #region DateTime

        /// <summary>
        /// The number of ticks per microsecond.
        /// </summary>
        public const int TicksPerMicrosecond = 10;
        /// <summary>
        /// The number of ticks per Nanosecond.
        /// </summary>
        public const int NanosecondsPerTick = 100;

        /// <summary>
        /// Gets the microsecond fraction of a DateTime.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Microseconds(this DateTime self)
        {
            return (int)Math.Floor(
               (self.Ticks
               % TimeSpan.TicksPerMillisecond)
               / (double)TicksPerMicrosecond);
        }
        /// <summary>
        /// Gets the Nanosecond fraction of a DateTime.  Note that the DateTime
        /// object can only store nanoseconds at resolution of 100 nanoseconds.
        /// </summary>
        /// <param name="self">The DateTime object.</param>
        /// <returns>the number of Nanoseconds.</returns>
        public static int Nanoseconds(this DateTime self)
        {
            return (int)(self.Ticks % TimeSpan.TicksPerMillisecond % TicksPerMicrosecond)
               * NanosecondsPerTick;
        }
        /// <summary>
        /// Adds a number of microseconds to this DateTime object.
        /// </summary>
        /// <param name="self">The DateTime object.</param>
        /// <param name="microseconds">The number of milliseconds to add.</param>
        public static DateTime AddMicroseconds(this DateTime self, int microseconds)
        {
            return self.AddTicks(microseconds * TicksPerMicrosecond);
        }
        /// <summary>
        /// Adds a number of nanoseconds to this DateTime object.  Note: this
        /// object only stores nanoseconds of resolutions of 100 seconds.
        /// Any nanoseconds passed in lower than that will be rounded using
        /// the default rounding algorithm in Math.Round().
        /// </summary>
        /// <param name="self">The DateTime object.</param>
        /// <param name="nanoseconds">The number of nanoseconds to add.</param>
        public static DateTime AddNanoseconds(this DateTime self, int nanoseconds)
        {
            return self.AddTicks((int)Math.Round(nanoseconds / (double)NanosecondsPerTick));
        }

        #endregion

        #region BinaryReader
        public static bool EOF(this BinaryReader binaryReader)
        {
            var bs = binaryReader.BaseStream;
            return (bs.Position == bs.Length);
        }
        #endregion


        #region BinaryFormatter

        public static byte[] SerializeToArray(this BinaryFormatter _BinaryFormatter, object obj)	//TODO: controllo se funziona da passaggio da T ( template ) a object
        {
            byte[] temp;
            using (MemoryStream ms = new MemoryStream())
            {
                _BinaryFormatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                temp = ms.ToArray();
            }
            return temp;
        }
        public static T DeserializeFromArray<T>(this BinaryFormatter _BinaryFormatter, byte[] data)
        {
            T temp;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data);
                ms.Seek(0, SeekOrigin.Begin);
                temp = (T)_BinaryFormatter.Deserialize(ms);
            }
            return temp;
        }


        #endregion


        #region Stream




        /// <summary>
        /// Permette di inviare un file tramite lo stream corrente
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="file"></param>
        public static void SendFile(this Stream stream, FilePlus file, int timeoutMillisecond = 0)
        {
            stream.WriteObject(file, timeoutMillisecond);
        }

        /// <summary>
        /// Permette di ricevere un file tramite lo stream corrente
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="file"></param>
        public static FilePlus ReceiveFile(this Stream stream, int timeoutMillisecond = 0)
        {
            return stream.ReadObject<FilePlus>(timeoutMillisecond);
        }

        public static void Write(this Stream stream, byte data, int TimeoutMillisecond = 0)
        {
            stream.Write(new byte[] { data }, TimeoutMillisecond);
        }

        public static void Remove(this MemoryStream stream, int numberOfBytesToRemove)
        {
            byte[] buf = stream.GetBuffer();
            Buffer.BlockCopy(buf, numberOfBytesToRemove, buf, 0, (int)stream.Length - numberOfBytesToRemove);
            stream.SetLength(stream.Length - numberOfBytesToRemove);

        }



        /// <summary>
        /// Permette di scrivere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="value">Array di byte da inviare</param>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static void Write(this Stream stream, byte[] value, int timeoutMillisecond = 0)
        {
            byte[] temp = value;

            if (timeoutMillisecond == 0)    //se è 0, aspetta all'infinito
            {
                stream.Write(temp, 0, temp.Length);
            }
            else
            {
                var result = stream.BeginWrite(temp, 0, temp.Length, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMillisecond));

                if (!success)
                {
                    throw new SocketException((int)SocketError.TimedOut);
                }
            }

        }


        /// <summary>
        /// Permette di scrivere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="value">Numero da inviare</param>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static void Write(this Stream stream, int value, int timeoutMillisecond = 0)
        {
            byte[] temp;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(value);
                    temp = ms.ToArray();
                }
            }

            stream.Write(temp, timeoutMillisecond);
        }

        /// <summary>
        /// Permette di scrivere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="value">Numero da inviare</param>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static void Write(this Stream stream, UInt64 value, int timeoutMillisecond = 0)
        {
            byte[] temp;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(value);
                    temp = ms.ToArray();
                }
            }

            stream.Write(temp, timeoutMillisecond);
        }

        /// <summary>
        /// Permette di scrivere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="value">Booleano da inviare</param>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static void Write(this Stream stream, bool value, int timeoutMillisecond = 0)
        {
            byte[] temp;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(value);
                    temp = ms.ToArray();
                }
            }


            stream.Write(temp, timeoutMillisecond);
        }

        /// <summary>
        /// Permette di scrivere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="value">Carattere da inviare</param>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static void Write(this Stream stream, char value, int timeoutMillisecond = 0)
        {
            byte[] temp;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(value);
                    temp = ms.ToArray();
                }
            }


            stream.Write(temp, timeoutMillisecond);
        }



        /// <summary>
        /// Permette di scrivere un oggetto sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="value">Booleano da inviare</param>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static void WriteObject<T>(this Stream stream, T obj, int timeoutMillisecond = 0)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            byte[] data = formatter.SerializeToArray(obj);

            UInt64 dim = (UInt64)data.Length;
            stream.Write(dim, timeoutMillisecond);  //invio la dimensione 
            if (stream.Read<Boolean>(timeoutMillisecond))   //aspetto la risposta
                stream.Write(data, timeoutMillisecond);     //invio il file

        }



        static Dictionary<Type, Func<BinaryReader, object>> StreamByteReaderTypeFunction = null;

        /*
        /// <summary>
        /// Permette di leggere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static T Read<T>(this Stream stream, int timeoutMillisecond = 0)
        {
           
            if (StreamByteReaderTypeFunction == null)
            {
                StreamByteReaderTypeFunction = new Dictionary<Type, Func<BinaryReader, object>>();
                StreamByteReaderTypeFunction.Add(typeof(Boolean), (BinaryReader reader) => { return reader.ReadBoolean(); });
                StreamByteReaderTypeFunction.Add(typeof(Byte), (BinaryReader reader) => { return reader.ReadByte(); });
                StreamByteReaderTypeFunction.Add(typeof(SByte), (BinaryReader reader) => { return reader.ReadSByte(); });
                StreamByteReaderTypeFunction.Add(typeof(Char), (BinaryReader reader) => { return reader.ReadChar(); });
                StreamByteReaderTypeFunction.Add(typeof(Int16), (BinaryReader reader) => { return reader.ReadInt16(); });
                StreamByteReaderTypeFunction.Add(typeof(UInt16), (BinaryReader reader) => { return reader.ReadUInt16(); });
                StreamByteReaderTypeFunction.Add(typeof(Int32), (BinaryReader reader) => { return reader.ReadInt32(); });
                StreamByteReaderTypeFunction.Add(typeof(UInt32), (BinaryReader reader) => { return reader.ReadUInt32(); });
                StreamByteReaderTypeFunction.Add(typeof(Int64), (BinaryReader reader) => { return reader.ReadInt64(); });
                StreamByteReaderTypeFunction.Add(typeof(UInt64), (BinaryReader reader) => { return reader.ReadUInt64(); });
                StreamByteReaderTypeFunction.Add(typeof(Single), (BinaryReader reader) => { return reader.ReadSingle(); });
                StreamByteReaderTypeFunction.Add(typeof(Double), (BinaryReader reader) => { return reader.ReadDouble(); });
                StreamByteReaderTypeFunction.Add(typeof(Decimal), (BinaryReader reader) => { return reader.ReadDecimal(); });
                StreamByteReaderTypeFunction.Add(typeof(String), (BinaryReader reader) => { return reader.ReadString(); });
            }

            //Controllo se T è valida e la posso parsare
            if (!StreamByteReaderTypeFunction.ContainsKey(typeof(T)))
            {
                throw new InvalidCastException("Tipo passato non riconosciuto, controlla la documentazione per i tipi di dati consentiti.");
            }
                

            //Dichiaro un array lungo quanto il tipo passato
            byte[] temp = new byte[System.Runtime.InteropServices.Marshal.SizeOf(default(T))];
            if (timeoutMillisecond == 0)    //se è 0, aspetta all'infinito
            {
                stream.Read(temp, 0, temp.Length);
            }
            else
            {
                var result = stream.BeginRead(temp, 0, temp.Length, null, null);        //inizio la lettura
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMillisecond));        //la interrompo dopo TOT millisecondi
                if (!success)       //se ha sforato il tempo sollevo un eccezione
                {
                    throw new SocketException((int)SocketError.TimedOut);
                }
            }


            //Usando un memory stream, carico tutto quello che ho letto in memoria, e lo passo ad un BinaryReader che converte nell'oggetto corretto
            T ret;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(temp,0,temp.Length);
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    ret=StreamByteReaderTypeFunction[typeof(T)](reader)._Cast<T>();
                }
            }
            return ret;          
        }
        */


        /// <summary>
        /// Permette di leggere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static byte[] Read(this Stream stream, int count, int timeoutMillisecond = 0)
        {

            //Dichiaro un array lungo quanto passato ( count ) 
            byte[] temp = new byte[count];
            if (timeoutMillisecond == 0)    //se è 0, aspetta all'infinito
            {
                stream.Read(temp, 0, temp.Length);
            }
            else
            {
                var result = stream.BeginRead(temp, 0, temp.Length, null, null);        //inizio la lettura
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMillisecond));        //la interrompo dopo TOT millisecondi
                if (!success)       //se ha sforato il tempo sollevo un eccezione
                {
                    throw new SocketException((int)SocketError.TimedOut);
                }
            }

            return temp;
        }

        /// <summary>
        /// Permette di leggere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// Tipi permessi: 
        /// Boolean, Byte, SByte, Char, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double, Decimal, String
        /// </summary>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static T Read<T>(this Stream stream, int timeoutMillisecond = 0)
        {
            if (StreamByteReaderTypeFunction == null)
            {
                StreamByteReaderTypeFunction = new Dictionary<Type, Func<BinaryReader, object>>();
                StreamByteReaderTypeFunction.Add(typeof(Boolean), (BinaryReader reader) => { return reader.ReadBoolean(); });
                StreamByteReaderTypeFunction.Add(typeof(Byte), (BinaryReader reader) => { return reader.ReadByte(); });
                StreamByteReaderTypeFunction.Add(typeof(SByte), (BinaryReader reader) => { return reader.ReadSByte(); });
                StreamByteReaderTypeFunction.Add(typeof(Char), (BinaryReader reader) => { return reader.ReadChar(); });
                StreamByteReaderTypeFunction.Add(typeof(char), (BinaryReader reader) => { return reader.ReadChar(); });
                StreamByteReaderTypeFunction.Add(typeof(Int16), (BinaryReader reader) => { return reader.ReadInt16(); });
                StreamByteReaderTypeFunction.Add(typeof(UInt16), (BinaryReader reader) => { return reader.ReadUInt16(); });
                StreamByteReaderTypeFunction.Add(typeof(Int32), (BinaryReader reader) => { return reader.ReadInt32(); });
                StreamByteReaderTypeFunction.Add(typeof(UInt32), (BinaryReader reader) => { return reader.ReadUInt32(); });
                StreamByteReaderTypeFunction.Add(typeof(Int64), (BinaryReader reader) => { return reader.ReadInt64(); });
                StreamByteReaderTypeFunction.Add(typeof(UInt64), (BinaryReader reader) => { return reader.ReadUInt64(); });
                StreamByteReaderTypeFunction.Add(typeof(Single), (BinaryReader reader) => { return reader.ReadSingle(); });
                StreamByteReaderTypeFunction.Add(typeof(Double), (BinaryReader reader) => { return reader.ReadDouble(); });
            }


            //Dichiaro un array lungo quanto il tipo passato
            int count = System.Runtime.InteropServices.Marshal.SizeOf(default(T));
            byte[] temp = stream.Read(count, timeoutMillisecond);       //recupero dallo stream i byte


            //Usando un memory stream, carico tutto quello che ho letto in memoria, e lo passo ad un BinaryReader che converte nell'oggetto corretto
            T ret;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(temp, 0, temp.Length);
                ms.Seek(0, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    ret = StreamByteReaderTypeFunction[typeof(T)](reader)._Cast<T>();
                }
            }
            return ret;

        }

        /// <summary>
        /// Permette di leggere un oggetto sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public static T ReadObject<T>(this Stream stream, int timeoutMillisecond = 0)
        {
            UInt64 dim = stream.Read<UInt64>(timeoutMillisecond);//aspetto la dimensione del file
            stream.Write(true, timeoutMillisecond); //invio l'OK
            byte[] data = stream.Read((int)dim, timeoutMillisecond); //aspetto il file


            BinaryFormatter formatter = new BinaryFormatter();
            T temp = formatter.DeserializeFromArray<T>(data);
            return temp;
        }




        #endregion


        #region TcpClient
        public static TcpState GetState(this TcpClient tcpClient)
        {
            var foo = IPGlobalProperties.GetIPGlobalProperties()
              .GetActiveTcpConnections()
              .FirstOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint));
            return foo != null ? foo.State : TcpState.Unknown;
        }
        public static bool IsConnected(this TcpClient tcpClient)
        {

            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                {
                    /* pear to the documentation on Poll:
                     * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                     * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                     * -or- true if data is available for reading; 
                     * -or- true if the connection has been closed, reset, or terminated; 
                     * otherwise, returns false
                     */

                    // Detect if client disconnected
                    if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        #endregion


        #region TreeNode
        public static TreeNode FirstParent(this TreeNode node)
        {
            if (node.Parent == null)
                return node;
            return node.Parent.FirstParent();
        }
        #endregion


        #region Rectangle

        public static PointF GetLocation(this RectangleF rect, System.Drawing.ContentAlignment Alignment = System.Drawing.ContentAlignment.TopLeft)
        {
            VerticalAlignment v = Alignment.GetVertical();
            HorizontalAlignment h = Alignment.GetHorizontal();

            float x = 0;
            if (h == HorizontalAlignment.Right)
            {
                x = rect.Size.Width;
            }
            else if (h == HorizontalAlignment.Center)
            {
                x = rect.Size.Width / 2;
            }


            float y = 0;
            if (v == VerticalAlignment.Bottom)
            {
                y = rect.Size.Height;
            }
            else if (v == VerticalAlignment.Center)
            {
                y = rect.Size.Height / 2;
            }


            return new PointF(rect.Left + x, rect.Top + y);
        }
        public static Point GetLocation(this Rectangle rect, System.Drawing.ContentAlignment Alignment = System.Drawing.ContentAlignment.TopLeft)
        {
            VerticalAlignment v = Alignment.GetVertical();
            HorizontalAlignment h = Alignment.GetHorizontal();

            int x = 0;
            if (h == HorizontalAlignment.Right)
            {
                x = rect.Size.Width;
            }
            else if (h == HorizontalAlignment.Center)
            {
                x = rect.Size.Width / 2;
            }


            int y = 0;
            if (v == VerticalAlignment.Bottom)
            {
                y = rect.Size.Height;
            }
            else if (v == VerticalAlignment.Center)
            {
                y = rect.Size.Height / 2;
            }


            return new Point(rect.Left + x, rect.Top + y);
        }
        public static VerticalAlignment GetVertical(this System.Drawing.ContentAlignment Alignment)
        {
            switch (Alignment)
            {
                case System.Drawing.ContentAlignment.BottomCenter:
                case System.Drawing.ContentAlignment.BottomLeft:
                case System.Drawing.ContentAlignment.BottomRight:
                    return VerticalAlignment.Bottom;
                case System.Drawing.ContentAlignment.TopCenter:
                case System.Drawing.ContentAlignment.TopLeft:
                case System.Drawing.ContentAlignment.TopRight:
                    return VerticalAlignment.Top;
                case System.Drawing.ContentAlignment.MiddleCenter:
                case System.Drawing.ContentAlignment.MiddleLeft:
                case System.Drawing.ContentAlignment.MiddleRight:
                    return VerticalAlignment.Center;

            }
            return VerticalAlignment.Top;
        }
        public static HorizontalAlignment GetHorizontal(this System.Drawing.ContentAlignment Alignment)
        {
            switch (Alignment)
            {
                case System.Drawing.ContentAlignment.BottomCenter:
                case System.Drawing.ContentAlignment.TopCenter:
                case System.Drawing.ContentAlignment.MiddleCenter:
                    return HorizontalAlignment.Center;
                case System.Drawing.ContentAlignment.TopLeft:
                case System.Drawing.ContentAlignment.MiddleLeft:
                case System.Drawing.ContentAlignment.BottomLeft:
                    return HorizontalAlignment.Left;
                case System.Drawing.ContentAlignment.MiddleRight:
                case System.Drawing.ContentAlignment.TopRight:
                case System.Drawing.ContentAlignment.BottomRight:
                    return HorizontalAlignment.Right;

            }
            return HorizontalAlignment.Left;
        }

        /// <summary>
        /// I riferimenti Top e Bottom seguono la logica cartesiana  ( zero Y in basso )  e non quella grafica PC
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="Alignment"></param>
        /// <returns></returns>
        public static PointF GetLocationCartesian(this RectangleF rect, System.Drawing.ContentAlignment Alignment = System.Drawing.ContentAlignment.TopLeft)
        {
            VerticalAlignment v = Alignment.GetVertical();
            HorizontalAlignment h = Alignment.GetHorizontal();

            float x = 0;
            if (h == HorizontalAlignment.Right)
            {
                x = rect.Size.Width;
            }
            else if (h == HorizontalAlignment.Center)
            {
                x = rect.Size.Width / 2;
            }


            float y = 0;
            if (v == VerticalAlignment.Top)
            {
                y = rect.Size.Height;
            }
            else if (v == VerticalAlignment.Center)
            {
                y = rect.Size.Height / 2;
            }


            return new PointF(rect.Left + x, rect.Top + y);
        }


        public static Rectangle Round(this RectangleF rect)
        {
            return Rectangle.Round(rect);
        }
        public static Rectangle Truncate(this RectangleF rect)
        {
            return Rectangle.Truncate(rect);
        }
        public static Rectangle Offset(this Rectangle rect, float offset)
        {
            return new Rectangle((int)(rect.X - offset), (int)(rect.Y - offset), (int)(rect.Width + (offset * 2)), (int)(rect.Height + (offset * 2)));
        }
        public static RectangleF Offset(this RectangleF rect, float offset)
        {
            return new RectangleF(rect.X - offset, rect.Y - offset, rect.Width + (offset * 2), rect.Height + (offset * 2));
        }

        /// <summary>
        /// Sottrae dal rettangolo corrente un altro rettangolo (INTERNO) creando 4 rettangoli che vanno a coprire l'area rimanente
        /// se non è interno ritorna un vettore vuoto;
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Rectangle[] Subract(this Rectangle rect, Rectangle other)
        {
            List<Rectangle> arr = new List<Rectangle>();

            if (rect.Contains(other))
            {
                Point rectBl = rect.GetLocation(System.Drawing.ContentAlignment.BottomLeft);
                Point otherBl = other.GetLocation(System.Drawing.ContentAlignment.BottomLeft);

                Point otherTr = other.GetLocation(System.Drawing.ContentAlignment.TopRight);

                Rectangle r1 = new Rectangle(rect.X, rect.Y, rect.Width, other.Y - rect.Y);
                Point r1Bl = r1.GetLocation(System.Drawing.ContentAlignment.BottomLeft);
                Point r1Br = r1.GetLocation(System.Drawing.ContentAlignment.BottomRight);
                Rectangle r2 = new Rectangle(r1Bl.X, r1Bl.Y,other.X- rect.X, other.Height);
                Point r2Bl = r2.GetLocation(System.Drawing.ContentAlignment.BottomLeft);
                Rectangle r3 = new Rectangle(r2Bl.X, r2Bl.Y, rect.Width, rectBl.Y- otherBl.Y);
                Point r3Tr = r3.GetLocation(System.Drawing.ContentAlignment.TopRight);
                Rectangle r4 = new Rectangle(otherTr.X, otherTr.Y, r3Tr.X-otherTr.X, other.Height);

                arr.Add(r1);
                arr.Add(r2);
                arr.Add(r3);
                arr.Add(r4);
            }

            return arr.ToArray();
        }


        #endregion

        #region Process
      
        public static IEnumerable<Process> GetChild(this Process process)
        {
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            return children;
        }

        #endregion

        #region MethodInfo
        public static Object Invoke(this MethodInfo mi, object obj, params object[] parameters)
        {
            return mi.Invoke(obj, parameters);
        }
        #endregion

        #region Resource

        public static object Get(this ResourceReader reader,string key)
        {
            return reader.Cast<DictionaryEntry>().Where((e) => { return (string)e.Key == key; }).FirstOrDefault().Value;
        }
        public static T Get<T>(this ResourceReader reader, string key)
        {
            return (T)reader.Cast<DictionaryEntry>().Where((e) => { return (string)e.Key == key; }).FirstOrDefault().Value;
        }
        

        #endregion


        //TODO: implementare gli altri ToPlus
        #region ToPlus

        public static PictureBoxPlus ToPlus(this PictureBox self)
        {
            PictureBoxPlus temp = new PictureBoxPlus()
            {
                _disableBitmapCreation = true,
                Bounds = self.Bounds,
                Size = self.Size,
                BackgroundImageLayout = self.BackgroundImageLayout,
                SizeMode = self.SizeMode
            };
            temp._disableBitmapCreation = false;
            temp.BackgroundImage = self.BackgroundImage;
            temp.Image = self.Image;
            return temp;
        }
        public static TcpClientPlus ToPlus(this TcpClient self)
        {
            return new TcpClientPlus(self);     
        }
        #endregion


    }




    public class TupleCombinationEqualityComparer<T> : IEqualityComparer<Tuple<T, T>>
    {
        public bool Equals(Tuple<T, T> x, Tuple<T, T> y)
        {
            bool equals = new HashSet<T>(new[] { x.Item1, x.Item2 }).SetEquals(new[] { y.Item1, y.Item2 });
            return equals;
        }

        public int GetHashCode(Tuple<T, T> obj)
        {
            return obj.Item1.GetHashCode() + obj.Item2.GetHashCode();
        }
    }
}
