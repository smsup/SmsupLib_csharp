using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SmsUp
{
    public class SmsUpLib
    {
        protected const string HOST = "https://www.smsup.es";
        protected const string URLsms = "/api/sms/";
        protected const string URLcreditos = "/api/creditos/";
        protected const string URLenvio = "/api/peticion/";

        protected string _usuario;
        protected string _clave;

        public SmsUpLib(string usuario, string clave)
        {
            this._usuario = usuario;
            this._clave = clave;
        }

        public string NuevoSms(string texto, string[] numeros, string fechaenvio, string referencia, string remitente)
        {
            Sms sms = new Sms
            {
                texto = texto,
                fecha = (fechaenvio ?? "NOW"),
                telefonos = numeros,
                referencia = referencia,
                remitente = remitente
            };
            JsonSerializerSettings settings = new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii };
            string post = JsonConvert.SerializeObject(sms, settings);

            return this.Enviar(URLsms, "POST", post);
        }

        public string EliminarSMS(string idsms)
        {
            return this.Enviar(URLsms + idsms + "/", "DELETE", "");
        }

        public string EstadosSMS(string idsms)
        {
            return this.Enviar(URLsms + idsms + "/", "GET", "");
        }

        public string CreditosDisponibles()
        {
            return this.Enviar(URLcreditos, "GET", "");
        }

        public string ResultadoPeticion(string referencia)
        {
            return this.Enviar(URLenvio + referencia + "/", "GET", "");
        }

        protected string Enviar(string url, string metodo, string body)
        {
            WebClient myClient = new WebClient
            {
                Headers = this.generarCabeceras(url, metodo, body)
            };
            string resul;
            try
            {
                if (metodo == "GET")
                    resul = Encoding.UTF8.GetString(myClient.DownloadData(new Uri(HOST + url)));
                else
                    resul = myClient.UploadString(new Uri(HOST + url), metodo, body);
            }
            catch (WebException ex)
            {
                WebResponse response = ex.Response;
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                resul = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                resul = ex.Message;
            }
            return resul;
        }

        protected WebHeaderCollection generarCabeceras(string url, string metodo, string body)
        {
            WebHeaderCollection cabeceras = new WebHeaderCollection();
            string fecha = DateTime.Now.ToString("o");
            cabeceras.Add("Sms-Date", fecha);
            string texto = metodo + url + fecha + body;
            cabeceras.Add("Firma", this._usuario + ":" + this.Hamac(texto));
            return cabeceras;
        }

        protected string Hamac(string texto)
        {
            var keyByte = Encoding.Default.GetBytes(this._clave);
            using (var hmacsha256 = new HMACSHA1(keyByte))
            {
                hmacsha256.ComputeHash(Encoding.Default.GetBytes(texto));
                return this.ByteToString(hmacsha256.Hash).ToLower();
            }
        }

        protected string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }
    }
}

