using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlosAgentSDK
{
    public class Funcoes
    {
        public void ExecuteSQL(String pCommand, Boolean pVerificaErro = true, SqlConnection conn = null)
        {
            SqlConnection gConn;
            if (conn == null)
            {
                gConn = (SqlConnection)GetSession("gConn", null);
            }
            else
            {
                gConn = conn;
            }

            bool checkconn = CheckConnSQL(gConn);
            if (!checkconn)
                return;

            SqlCommand cmd = new SqlCommand(pCommand, gConn);
            cmd.CommandTimeout = 999999999;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool CheckConnSQL(SqlConnection gConn, SqlDataReader gRead = null)
        {
            SetSession("gsERRO_DISPLAY", "");
            SetSession("gbERRO_STOP", false);

            if (gConn == null)
            {
                gConn = new SqlConnection();
                gConn.ConnectionString = GetSession("gConnString", "").ToString();
                if (gConn.ConnectionString == "") return false;

                Reconnection(gConn);
                SetSession("gConn", gConn);
            }

            if (gRead != null)
                gRead.Close();
            SetSession("gRead", null);

            if ((gConn != null) && (gConn.State == ConnectionState.Closed))
                Reconnection(gConn);

            return true;
        }

        public void Connection(String pIP, String pDatabase, String pUsu, String pSenha, String pTerminal, String pInstancia, String pPorta)
        {
            Disconnect();
            SqlConnection gConn = new SqlConnection();

            gConn.Close();
            gConn.Dispose();

            string pDataSource = pIP;

            if (pInstancia != "")
                pDataSource += "\\" + pInstancia;
            if (pPorta != "")
                pDataSource += "," + pPorta;

            SetSession("sDATABASE", pDatabase);

            gConn.ConnectionString = "Password=" + pSenha + ";" +
                                     "Persist Security Info=True;" +
                                     "User ID=" + pUsu + ";" +
                                     "Initial Catalog=" + pDatabase + ";" +
                                     "Data Source=" + pDataSource + ";" +
                                     "Workstation ID=" + pTerminal + ";" +
                                     "MultipleActiveResultSets=True;" +
                                     "Connection Timeout=30;" +
                                     "Pooling=false;" +
                                     "Max Pool Size=5000;";

            SetCache("gsConn", gConn.ConnectionString);
            SetSession("gConnString", gConn.ConnectionString);

            Reconnection(gConn);

            SetSession("gConn", gConn);
        }

        public SqlConnection Reconnection(SqlConnection gConn)
        {
            try
            {
                gConn.Open();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return gConn;
        }

        public void SetCache(string pVariavel, object psVariavel)
        {
            if (psVariavel != null)
            {
                HttpRuntime.Cache.Insert(pVariavel, psVariavel);
            }
        }

        public void Disconnect()
        {
            try
            {
                SqlConnection gConn = (SqlConnection)GetSession("gConn");
                SqlConnection.ClearPool(gConn);
                gConn.Close();
                gConn.Dispose();
            }
            catch
            {

            }
        }

        public string ValorSQL(string pSELECT, SqlConnection conn = null, SqlDataReader reader = null)
        {
            string valor = "";
            SqlDataReader read;
            read = Abre_Data(pSELECT, true, conn, reader);
            if (read != null)
            {
                if (read.HasRows)
                {
                    read.Read();
                    valor = read.GetValue(0).ToString();
                    read.Close();
                }
            }
            return valor;
        }

        public SqlDataReader Abre_Data(String pSelect, Boolean pVerificaErro = true, SqlConnection conn = null, SqlDataReader reader = null)
        {
            SqlDataReader gRead = reader == null ? (SqlDataReader)GetSession("gRead") : reader;
            SqlConnection gConn = conn == null ? (SqlConnection)GetSession("gConn") : conn;

            bool checkconn = CheckConnSQL(gConn, gRead);
            if (!checkconn)
                return gRead;

            SqlCommand cmd = new SqlCommand(pSelect, gConn);
            cmd.CommandTimeout = 999999999;
            SqlDataReader read;

            try
            {
                read = cmd.ExecuteReader();
                SetSession("gRead", read);
                return read;
            }
            catch
            {
            }
            finally
            {
                cmd.Dispose();
            }
            return null;
        }

        public void Executa_Data(String pCommand, Boolean pVerificaErro = true, SqlConnection conn = null)
        {
            SqlConnection gConn = conn == null ? (SqlConnection)GetSession("gConn", null) : conn;

            bool checkconn = CheckConnSQL(gConn);
            if (!checkconn)
                return;

            SqlCommand cmd = new SqlCommand(pCommand, gConn);
            cmd.CommandTimeout = 999999999;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                ExceptionSQLRun(pCommand, E, "Executa_Data");
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public void Executa_Data(SqlCommand cmd, Boolean pVerificaErro = true)
        {
            cmd.CommandTimeout = 999999999;
            string query = cmd.CommandText + ",";
            foreach (SqlParameter p in cmd.Parameters)
                query = query.Replace(p.ParameterName + (query.Contains(p.ParameterName + ",") ? "," : " "), "'" + p.Value.ToString() + "'" + (query.Contains(p.ParameterName + ",") ? "," : " "));
            query = query.Substring(0, query.Length - 1);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                ExceptionSQLRun(query, E, "Executa_Data");
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public void ExceptionSQLRun(string sql, Exception E, string function)
        {
            String sERRO;
            int nPIPE;
            int nVIRGULA;
            string gsERRO_DISPLAY = "";
            string gsERRO_SQL = "";

            SetSession("sLastSQLError", "Erro:" + E.Message.ToString() + Environment.NewLine + Environment.NewLine + sql);

            nPIPE = E.Message.ToString().IndexOf("|");
            gsERRO_DISPLAY = "";
            if (nPIPE > 0)
            {
                gsERRO_SQL = E.Message.ToString();
                sERRO = E.Message.ToString();

                nVIRGULA = sERRO.IndexOf(",");
                if (nVIRGULA > 0)
                {
                    while (nVIRGULA > 0)
                    {
                        if (gsERRO_DISPLAY == "")
                        {
                            gsERRO_DISPLAY = sERRO.Substring(0, nPIPE);
                            sERRO = sERRO.Substring(nVIRGULA + 1, sERRO.Length - (nVIRGULA + 1));
                        }
                        else
                        {
                            gsERRO_DISPLAY = gsERRO_DISPLAY + " <br> " + sERRO.Substring(0, nPIPE);
                            sERRO = sERRO.Substring(nVIRGULA + 1, sERRO.Length - (nVIRGULA + 1));
                        }

                        nVIRGULA = sERRO.IndexOf(",");

                        nPIPE = sERRO.IndexOf("|");
                    }
                    if (nPIPE > 0)
                    {
                        gsERRO_DISPLAY = gsERRO_DISPLAY + " <br> " + sERRO.Substring(0, nPIPE);
                    }
                }
                else
                {
                    gsERRO_DISPLAY = sERRO.Substring(0, nPIPE);
                }
            }
            else
            {
                gsERRO_DISPLAY = E.Message.ToString();
                gsERRO_SQL = "";
            }

            SetSession("gsERRO_SQL", gsERRO_SQL);
            SetSession("gsERRO_DISPLAY", gsERRO_DISPLAY);

            VerificaErro();
        }

        public bool VerificaErro()
        {
            bool gbERRO_STOP = false;
            string gsERRO_DISPLAY = GetSession("gsERRO_DISPLAY", "").ToString();

            if (gsERRO_DISPLAY.Trim() != "")
            {

                gsERRO_DISPLAY = gsERRO_DISPLAY.Replace("\"", "");
                gsERRO_DISPLAY = gsERRO_DISPLAY.Replace("\r\n", "<br />");
                gsERRO_DISPLAY = gsERRO_DISPLAY.Replace("'", "\'");


                if (gsERRO_DISPLAY.IndexOf("datetime") >= 0)
                {
                    Console.WriteLine("Erro", "Data inválida!", "E");
                }
                else if (gsERRO_DISPLAY.IndexOf("chave duplicada") >= 0)
                {
                    Console.WriteLine("Este registro já existe!", "Não foi possível inserir o dado!", "i");
                }
                else if (gsERRO_DISPLAY.ToString().Contains("DELETE conflitou com a restrição"))
                {
                    Console.WriteLine("Não foi possível realizar a exclusão!", "Não é possível completar essa exclusão.\nMotivo: Esse dado está vinculado a alguma outra informação que necessita desse dado para existir.", "i");
                }
                else if (gsERRO_DISPLAY.ToString().Contains("INSERT conflitou com a restrição do FOREIGN"))
                {
                    Console.WriteLine("Não foi possível inserir esse dado!", "Esse dado já existe", "i");
                }
                else
                {
                    Console.WriteLine("Atenção", gsERRO_DISPLAY, "i");
                }

                gbERRO_STOP = true;
            }
            SetSession("gbERRO_STOP", gbERRO_STOP);
            return gbERRO_STOP;
        }

        public string Replicate(string caractere, int quantidade)
        {
            string resultado = "";
            for (int i = 0; i < quantidade; i++)
                resultado += caractere;
            return resultado;
        }

        public string[] DateArray()
        {
            DateTime data = DateTime.Now;
            string[] dt = { data.Day.ToString(), data.Month.ToString(), data.Year.ToString() };
            string[] tm = { data.Hour.ToString(), data.Minute.ToString(), data.Second.ToString(), data.Millisecond.ToString() };
            dt[0] = Replicate("0", 2 - dt[0].Length) + dt[0];
            dt[1] = Replicate("0", 2 - dt[1].Length) + dt[1];
            dt[2] = Replicate("0", 4 - dt[2].Length) + dt[2];
            tm[0] = Replicate("0", 2 - tm[0].Length) + tm[0];
            tm[1] = Replicate("0", 2 - tm[1].Length) + tm[1];
            tm[2] = Replicate("0", 2 - tm[2].Length) + tm[2];
            tm[3] = Replicate("0", 3 - tm[3].Length) + tm[3];
            return new string[] { dt[0], dt[1], dt[2], tm[0], tm[1], tm[2], tm[3] };
        }

        public bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists) return false;

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();
                }
                WaitGarbage();
            }
            return false;
        }

        public void WaitGarbage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void LoadCombo(DropDownList ddl, String pSelect, String pID, String pCampo, String pHeader, bool saveINDEX = false)
        {
            SqlDataReader read;

            int position = -1;
            if (saveINDEX) position = ddl.SelectedIndex;

            ddl.Items.Clear();
            ListItem li = new ListItem();

            li.Text = pHeader;
            li.Value = "0";

            ddl.Items.Add(li);

            read = Abre_Data(pSelect);

            if (read != null)
            {
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        ListItem listItem = new ListItem();

                        listItem.Text = read[pCampo].ToString();
                        listItem.Value = read[pID].ToString();

                        ddl.Items.Add(listItem);
                    }
                }
                read.Close();
            }

            if ((saveINDEX) && (position > -1)) ddl.SelectedIndex = position;
        }

        public void SetSession(string pVariavel, object pValor)
        {
            try
            {
                HttpContext.Current.Session[pVariavel] = pValor;
                SetListSession(pVariavel);
            }
            catch { }
        }

        public void SetListSession(string variante)
        {
            if (variante == "_sessionlist_") return;
            string[] lista = (string[])GetSession("_sessionlist_", null);
            if ((lista == null) || (!lista.Contains(variante)))
            {
                string[] listanew = new string[(lista != null ? lista.Length : 0) + 1];
                if (lista != null)
                    lista.CopyTo(listanew, 0);
                listanew[(lista != null ? lista.Length : 0)] = variante;
                SetSession("_sessionlist_", listanew);
            }
        }

        public void js(String pCOMANDO, Page gpPagina = null)
        {
            if (gpPagina == null)
                gpPagina = (Page)GetSession("gpPagina");
            if (gpPagina == null)
                return;
            if (pCOMANDO.Substring(pCOMANDO.Length - 1, 1) != ";") { pCOMANDO = pCOMANDO + ";"; }
            ScriptManager.RegisterStartupScript(gpPagina, gpPagina.GetType(), Guid.NewGuid().ToString(), pCOMANDO, true);
        }

        public object GetSession(string pVariavel, object pTipo = null)
        {
            object resultado = new object();
            try
            {
                resultado = HttpContext.Current.Session[pVariavel];
            }
            catch { }

            if (resultado == null)
                return pTipo;
            return resultado;
        }

        public String EncodeISO(String pVALOR, bool pISO = true)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(pVALOR);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);

            if (pISO)
            {
                return iso.GetString(isoBytes);
            }
            else
            {
                return utf8.GetString(utfBytes);
            }
        }

        public String COBCRPTO(String pSenha)
        {
            String[] Simbolos = new String[4];
            String sValor;
            String Result = String.Empty;

            int i;

            Simbolos[0] = "ABCDEFGHIJLMNOPQRSTUVXZYWK ~!@#$%^&*()";
            Simbolos[1] = "5Üø£ÂÀ©Øû×Æôöò»ƒçêùÿÙýÄÅÉúñÑªº¿®¬¼ëèïæÁ";
            Simbolos[2] = "abcdefghijlmnopqrstuvxzywk1234567890";
            Simbolos[3] = "Çüé¾¶§áâäàþîì¡åíó÷¸°¨·¹ÎÏ-+ÌÓß³²Õµ«½";

            for (i = 0; i < pSenha.Length; i++)
            {
                sValor = pSenha[i].ToString();

                for (int x = 0; x < Simbolos.Length; x++)
                    if ((Simbolos[x].IndexOf(sValor) >= 0))
                    {
                        sValor = Simbolos[(NumeroPar(x) ? (x + 1) : (x - 1))][Simbolos[x].IndexOf(sValor)].ToString();
                        break;
                    }

                Result += sValor;
            }

            return Result;
        }

        public bool NumeroPar(int numero)
        {
            return IsEven(numero);
        }

        public bool IsEven(int numero)
        {
            decimal dividido = (decimal)numero / (decimal)2;
            return (dividido == Math.Round(dividido));
        }

        public string MensagemTrataTexto(string txt, string tipo = "STRING")
        {
            txt = txt.Replace("\\", "\\\\"); //Barra
            txt = txt.Replace("'", "\\'"); //Aspas
            switch (tipo)
            {
                case "STRING": txt = txt.Replace("\r\n", "'+char13+char10+'").Replace("\n", "'+char10+'"); break; //Quebra de Linha
                case "HTML": txt = txt.Replace("\r\n", "<br />").Replace("\n", "<br />"); break;
            }
            return txt;
        }

        public HttpWebRequest CreateWebRequest(String pWS_URL, String pWS_URL_ACTION, String pWS_USER, String pWS_PASSWORD)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(pWS_URL);

            if (pWS_URL_ACTION == "")
            {
                webRequest.Headers.Add(@"SOAP:Action");
            }
            else
            {
                webRequest.Headers.Add("SOAPAction", pWS_URL_ACTION);
            }

            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            if (pWS_USER != "")
            {
                string sLogin = pWS_USER + ":" + pWS_PASSWORD;
                sLogin = Convert.ToBase64String(new ASCIIEncoding().GetBytes(sLogin));
                CredentialCache ccLogin = new CredentialCache();

                webRequest.PreAuthenticate = true;
                webRequest.Credentials = ccLogin;
                webRequest.Headers.Add("Authorization", "Basic " + sLogin);
            }

            return webRequest;
        }

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) &&
                (!strInput.StartsWith("[") || !strInput.EndsWith("]")))
            {
                return false;
            }
            try
            {
                JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public bool ExisteRegistro(string pSELECT)
        {
            SqlDataReader read;
            bool existe = false;
            read = Abre_Data(pSELECT);
            if (read != null)
            {
                existe = read.HasRows;
                read.Close();
            }
            return existe;
        }
    }
}