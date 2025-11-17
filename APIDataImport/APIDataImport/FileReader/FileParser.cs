// Decompiled with JetBrains decompiler
// Type: APIDataImport.FileReader.FileParser
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace APIDataImport.FileReader
{
  public static class FileParser
  {
    public static void DeconstructFile(string fileName, string folderPath)
    {
      string fileName1 = string.Format("{0}\\Temp\\MuestrasExec_{1:dd-MM-yyyy_hhmmss}.txt", (object) folderPath, (object) DateTime.Now);
      string fileName2 = string.Format("{0}\\Temp\\ResultadosExec_{1:dd-MM-yyyy_hhmmss}.txt", (object) folderPath, (object) DateTime.Now);
      FileInfo fileInfo1 = new FileInfo(fileName1);
      FileInfo fileInfo2 = new FileInfo(fileName2);
      SortedList<string, WEBResultados> sortedList = new SortedList<string, WEBResultados>();
      if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);
      using (StreamWriter text1 = fileInfo1.CreateText())
      {
        using (StreamWriter text2 = fileInfo2.CreateText())
        {
          bool flag1 = false;
          bool flag2 = false;
          foreach (string readLine in File.ReadLines(fileName))
          {
            if (!readLine.Trim().Equals("SIN DATOS"))
            {
              if (readLine.Trim().Equals("---DATOS MUESTRAS---"))
              {
                flag1 = true;
                flag2 = false;
              }
              else if (readLine.Trim().Equals("---DATOS ESTACIONES---") || readLine.Trim().Equals("---DATOS METODOS---"))
              {
                flag1 = false;
                flag2 = false;
              }
              else if (readLine.Trim().Equals("---DATOS RESULTADOS---"))
              {
                flag1 = false;
                flag2 = true;
              }
              else if (readLine.Contains(";"))
              {
                if (flag1)
                {
                  if (!readLine.Trim().Contains("ID_MUESTRA"))
                  {
                    List<string> list = ((IEnumerable<string>) readLine.Split(';')).ToList<string>();
                    WEBMuestras webMuestras = new WEBMuestras()
                    {
                      ID_MUESTRA = int.Parse(list[0].Trim()),
                      NUMERO_MUESTRA = list[1].Trim(),
                      SUB_MUESTRA = int.Parse(list[2].Trim()),
                      SUSTITUTA = int.Parse(list[3].Trim()),
                      FECHA_MUESTREO = list[4].Trim(),
                      FECHA_ARRIBO_LABORATORIO = list[5].Trim(),
                      FECHA_EMISION_INFORME = list[6].Trim(),
                      NOMBRE_ESTACION = list[7].Trim(),
                      PROYECTO = list[8].Trim(),
                      EMPRESA_CONTRATANTE = list[9].Trim(),
                      EMPRESA_SOLICITANTE = list[10].Trim(),
                      TIPO_MUESTRA = list[11].Trim(),
                      ESTADO = list[12].Trim(),
                      NUMERO_PROCESO = list[13].Trim(),
                      GRUPO_MUESTRA = list[14].Trim()
                    };
                    text1.WriteLine(JsonConvert.SerializeObject((object) webMuestras));
                  }
                }
                else if (flag2 && !readLine.Trim().Contains("ID_MUESTRA"))
                {
                  List<string> list = ((IEnumerable<string>) readLine.Split(';')).ToList<string>();
                  WEBResultados webResultados = new WEBResultados()
                  {
                    ID_MUESTRA = int.Parse(list[0].Trim()),
                    ID_METODO = int.Parse(list[1].Trim()),
                    CAS = list[2].Trim(),
                    ANALITO = list[3].Trim(),
                    RESULTADO = list[4].Trim(),
                    UNIDAD = list[5].Trim(),
                    FECHA_ANALISIS = list[6].Trim(),
                    LIMITE_DETECCION = list[7].Trim(),
                    LIMITE_CUANTIFICACION = list[8].Trim(),
                    INCERTIDUMBRE = list[9].Trim(),
                    ID_NORMATIVA = list[10].Trim()
                  };
                  sortedList.Add(webResultados.ID_MUESTRA.ToString() + webResultados.ANALITO, webResultados);
                }
              }
            }
            else
              break;
          }
          foreach (WEBResultados webResultados in (IEnumerable<WEBResultados>) sortedList.Values)
            text2.WriteLine(JsonConvert.SerializeObject((object) webResultados));
        }
      }
      File.Move(fileName, fileName.Replace(folderPath, folderPath + "Processed\\"));
    }
  }
}
