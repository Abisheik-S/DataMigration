// Decompiled with JetBrains decompiler
// Type: APIDataImport.WebApiTask
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using APIDataImport.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Net;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;
using WEBApiRequestObjectModel;

namespace APIDataImport
{
  [SampleManagerTask("ApiDataDownload", "WorkflowCallback")]
  public class WebApiTask : SampleManagerTask
  {
    protected virtual void SetupTask()
    {
      base.SetupTask();
      this.Library.Utils.FlashMessage(this.GetMonitoreoAmbientalData(), "Carga Datos WEB API");
      this.Exit();
    }

    public string GetMonitoreoAmbientalData()
    {
      if (string.IsNullOrEmpty(this.Context.TaskParameterString))
        return "No Parameters defined for Task";
      string savedToken = this.GetSavedToken();
      if (savedToken.Contains("Error"))
        return savedToken;
      string[] strArray = this.Context.TaskParameterString.Split(',');
      string str1 = strArray[0].Trim();
      string str2 = strArray[1].Trim();
      this.Logger.Error((object) ("The Data imported started from " + str1 + " to " + str2));
      IQuery query = this.EntityManager.CreateQuery<ConfigHeader>();
      query.AddEquals("Identity", (object) "WEB_API_ENV_MON_URL");
      IEntityCollection ientityCollection = this.EntityManager.Select(query, true);
      if (((ICollection) ientityCollection).Count <= 0)
        return "No URL defined for Task";
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(((ConfigHeaderBase) ientityCollection.GetFirst()).Value);
      httpWebRequest.Method = "POST";
      httpWebRequest.ContentType = "application/json";
      httpWebRequest.Accept = "application/json";
      httpWebRequest.PreAuthenticate = true;
      httpWebRequest.Headers.Add("Authorization", "Bearer " + savedToken);
      WEBFiltro webFiltro = new WEBFiltro()
      {
        tipo_fecha = 1,
        fecha_desde = str1 + " 00:00:00",
        fecha_hasta = str2 + " 23:59:59"
      };
      using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        streamWriter.Write(JsonConvert.SerializeObject((object) webFiltro));
        streamWriter.Flush();
        streamWriter.Close();
      }
      try
      {
        using (WebResponse response = httpWebRequest.GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            if (responseStream == null)
              return (string) null;
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
              string end = streamReader.ReadToEnd();
              if (end == "[]")
                return (string) null;
              this.CreateResultFile(JsonConvert.DeserializeObject<WEBMonitoreoAmbientalData>(end));
            }
          }
        }
        return "Éxito al cargar los Datos";
      }
      catch (WebException ex)
      {
        return "Error: " + ex.ToString();
      }
    }

    private string GetSavedToken()
    {
      IQuery query = this.EntityManager.CreateQuery<ConfigHeader>();
      query.AddEquals("Identity", (object) "WEB_API_TOKEN");
      IEntityCollection ientityCollection = this.EntityManager.Select(query, true);
      if (((ICollection) ientityCollection).Count <= 0)
        return "Error: Token Configuration item is not Created.";
      ConfigHeader first = (ConfigHeader) ientityCollection.GetFirst();
      string savedToken = ((ConfigHeaderBase) first).Value;
      if (!string.IsNullOrEmpty(savedToken))
        return savedToken;
      string newToken = this.GetNewToken();
      if (string.IsNullOrEmpty(newToken))
        return "Error: Error creating new Token. Check URL Settings and Username/Password Configuration";
      ((ConfigHeaderBase) first).Value = newToken;
      this.EntityManager.Transaction.Add((IEntity) first);
      this.EntityManager.Commit();
      return newToken;
    }

    private string GetNewToken()
    {
      WEBLoginResponse loginInfo = this.GetLoginInfo();
      return loginInfo != null ? loginInfo.data.token : "";
    }

    private WEBLoginResponse GetLoginInfo()
    {
      IQuery query1 = this.EntityManager.CreateQuery<ConfigHeader>();
      IQuery query2 = this.EntityManager.CreateQuery<ConfigHeader>();
      query1.AddEquals("Identity", (object) "WEB_API_USERNAME");
      query2.AddEquals("Identity", (object) "WEB_API_PASSWORD");
      IEntityCollection ientityCollection1 = this.EntityManager.Select(query1, true);
      IEntityCollection ientityCollection2 = this.EntityManager.Select(query2, true);
      if (((ICollection) ientityCollection1).Count <= 0 || ((ICollection) ientityCollection2).Count <= 0)
        return (WEBLoginResponse) null;
      Conex conex = new Conex()
      {
        email = ((ConfigHeaderBase) ientityCollection1.GetFirst()).Value,
        password = ((ConfigHeaderBase) ientityCollection2.GetFirst()).Value
      };
      IQuery query3 = this.EntityManager.CreateQuery<ConfigHeader>();
      query3.AddEquals("Identity", (object) "WEB_API_LOGIN_URL");
      IEntityCollection ientityCollection3 = this.EntityManager.Select(query3, true);
      if (((ICollection) ientityCollection3).Count <= 0)
        return (WEBLoginResponse) null;
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(((ConfigHeaderBase) ientityCollection3.GetFirst()).Value);
      httpWebRequest.Method = "POST";
      httpWebRequest.ContentType = "application/json";
      httpWebRequest.Accept = "application/json";
      using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        streamWriter.Write(JsonConvert.SerializeObject((object) conex));
        streamWriter.Flush();
        streamWriter.Close();
      }
      try
      {
        using (WebResponse response = httpWebRequest.GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            if (responseStream == null)
              return (WEBLoginResponse) null;
            using (StreamReader streamReader = new StreamReader(responseStream))
              return JsonConvert.DeserializeObject<WEBLoginResponse>(streamReader.ReadToEnd());
          }
        }
      }
      catch (WebException ex)
      {
        return (WEBLoginResponse) null;
      }
    }

    private void CreateResultFile(WEBMonitoreoAmbientalData monitoreo)
    {
      IQuery query = this.EntityManager.CreateQuery<ConfigHeader>();
      query.AddEquals("Identity", (object) "WEB_API_FILEPATH");
      IEntityCollection ientityCollection = this.EntityManager.Select(query, true);
      if (((ICollection) ientityCollection).Count <= 0)
        return;
      string fileName = ((ConfigHeaderBase) ientityCollection.GetFirst()).Value + "\\Response_" + DateTime.Now.ToString("dd-MM-yyyy_hhmmss") + ".txt";
      string globalString = this.Library.Environment.GetGlobalString("WEB_API_FILEPATH");
      FileInfo fileInfo = new FileInfo(fileName);
      if (!Directory.Exists(globalString))
        Directory.CreateDirectory(globalString);
      using (StreamWriter text = fileInfo.CreateText())
      {
        if (monitoreo == null)
        {
          text.WriteLine("SIN DATOS");
        }
        else
        {
          text.WriteLine("---DATOS MUESTRAS---");
          text.WriteLine("ID_MUESTRA;NUMERO_MUESTRA;SUB_MUESTRA;SUSTITUTA;FECHA_MUESTREO;FECHA_ARRIBO_LABORATORIO;FECHA_EMISION_INFORME;NOMBRE_ESTACION;PROYECTO;EMPRESA_CONTRATANTE;EMPRESA_SOLICITANTE;TIPO_MUESTRA;ESTADO;NUMERO_PROCESO;GRUPO_MUESTRA");
          if (monitoreo.Muestras == null)
            return;
          foreach (WEBMuestras muestra in monitoreo.Muestras)
            text.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}", (object) muestra.ID_MUESTRA, (object) muestra.NUMERO_MUESTRA, (object) muestra.SUB_MUESTRA, (object) muestra.SUSTITUTA, (object) muestra.FECHA_MUESTREO, (object) muestra.FECHA_ARRIBO_LABORATORIO, (object) muestra.FECHA_EMISION_INFORME, (object) muestra.NOMBRE_ESTACION, (object) muestra.PROYECTO, (object) muestra.EMPRESA_CONTRATANTE, (object) muestra.EMPRESA_SOLICITANTE, (object) muestra.TIPO_MUESTRA, (object) muestra.ESTADO, (object) muestra.NUMERO_PROCESO, (object) muestra.GRUPO_MUESTRA));
          text.WriteLine("---DATOS ESTACIONES---");
          text.WriteLine("NOMBRE_ESTACION;LATITUD_NORTE;LONGITUD_ESTE;HEMISFERIO;ZONA;DESCRIPCION");
          foreach (WEBEstaciones estacione in monitoreo.Estaciones)
            text.WriteLine(estacione.NOMBRE_ESTACION + ";" + estacione.LATITUD_NORTE + ";" + estacione.LONGITUD_ESTE + ";" + estacione.HEMISFERIO + ";" + estacione.ZONA + ";" + estacione.DESCRIPCION);
          text.WriteLine("---DATOS METODOS---");
          text.WriteLine("ID_METODO;DESCRIPCION_METODO;REFERENCIA;AREA;HOLDING_TIME");
          foreach (WEBMetodos metodo in monitoreo.Metodos)
            text.WriteLine(string.Format("{0};{1};{2};{3};{4}", (object) metodo.ID_METODO, (object) metodo.DESCRIPCION_METODO, (object) metodo.REFERENCIA, (object) metodo.AREA, (object) metodo.HOLDING_TIME));
          text.WriteLine("---DATOS RESULTADOS---");
          text.WriteLine("ID_MUESTRA;ID_METODO;CAS;ANALITO;RESULTADO;UNIDAD;FECHA_ANALISIS;LIMITE_DETECCION;LIMITE_CUANTIFICACION;INCERTIDUMBRE;ID_NORMATIVA");
          foreach (WEBResultados resultado in monitoreo.Resultados)
            text.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", (object) resultado.ID_MUESTRA, (object) resultado.ID_METODO, (object) resultado.CAS, (object) resultado.ANALITO, (object) resultado.RESULTADO, (object) resultado.UNIDAD, (object) resultado.FECHA_ANALISIS, (object) resultado.LIMITE_DETECCION, (object) resultado.LIMITE_CUANTIFICACION, (object) resultado.INCERTIDUMBRE, (object) resultado.ID_NORMATIVA));
        }
      }
    }
  }
}
