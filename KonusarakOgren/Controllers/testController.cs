using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using KonusarakOgren.Models;
using Microsoft.AspNetCore.Mvc;

namespace KonusarakOgren.Controllers
{
    public class testController : Controller
    {
        SQLiteConnection baglan;
        SQLiteCommand cmd;
        string sorgu;
        public void sqlBaglanti()
        {
           baglan = new SQLiteConnection();
            baglan.ConnectionString = ("Data Source=Models/KonusarakOgren.db");
            baglan.Open();
        }
        public IActionResult testleriGetir()
        {
            sqlBaglanti();
           sorgu = "SELECT id,baslik FROM text ";
           cmd = new SQLiteCommand(sorgu, baglan);
           
            List<soru> baslikList = new List<soru>();
            using (var reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                  
                    var list = new soru();
                    list.id = reader.GetInt32(0);
                    list.baslik = reader.GetString(1);
                    
                    baslikList.Add(list);


                }

            }
                return View(baslikList);
        }
        public IActionResult sil(int id)
        {
            sqlBaglanti();
            sorgu = "delete from text where id=@id";
            cmd = new SQLiteCommand(sorgu,baglan);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            sorgu = "delete from question where textId=@textId";
            cmd = new SQLiteCommand(sorgu, baglan);
            cmd.Parameters.AddWithValue("@textId", id);
            cmd.ExecuteNonQuery();
            return RedirectToAction("testleriGetir");
        }

        public IActionResult soru(int id)
        {
            sqlBaglanti();
            sorgu = "select * from question q inner join text t on q.textId=t.id where t.id=@textId ";
            cmd = new SQLiteCommand(sorgu, baglan);
            cmd.Parameters.AddWithValue("@textId", id);
            List<soruGetir> soruList = new List<soruGetir>();
            using (var reader = cmd.ExecuteReader())
            {
                
                while(reader.Read())
                {
                    
                    var model = new soruGetir();
                  
                    model.sorular = reader.GetString(2);
                    model.secenekA = reader.GetString(3);
                    model.secenekB = reader.GetString(4);
                    model.secenekC = reader.GetString(5);
                    model.secenekD = reader.GetString(6);
                    model.dogruCevap = reader.GetString(7);
                    ViewBag.baslik = reader.GetString(9);
                    ViewBag.icerik = reader.GetString(10);
                    soruList.Add(model);
                    
                }
            }
            return View(soruList);
        }
    }
}