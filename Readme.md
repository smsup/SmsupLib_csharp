SmsupLib
========

Clase que facilita el uso de la api de smsup.es para el envio de sms.

- [NuGet Package](https://www.nuget.org/packages/SmsupLib/)

Uso
---

```csharp
	SmsUpLib smsup = new SmsUpLib('TU_ID_USUARIO','TU_CLAVE_SECRETA');
	textoResultado = smsup.NuevoSms("Texto del mensaje", new string[] { "600000000" }, null, "", "Remitente");
```