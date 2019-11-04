using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using KonusarakOgren.Models;
using Microsoft.AspNetCore.Mvc;

namespace KonusarakOgren.Controllers
{
    public class LoginController : Controller
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
        public IActionResult giris()
        {
            //Kullanıcı adı:admin sifre:konusarakogren
            return View();
        }

        [HttpPost]
        public IActionResult giris(Login kullanici)
        {
            sqlBaglanti();
            sorgu = "select * from users where userName=@userName AND password=@password";
            cmd = new SQLiteCommand(sorgu, baglan);
            cmd.Parameters.AddWithValue("@userName", kullanici.userName);
            cmd.Parameters.AddWithValue("@password", kullanici.sifre);
            int id = 0; 
            using (var reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    id = reader.GetInt32(0);
                }
            }
            if(id!=0)
            {
                return RedirectToAction("testleriGetir", "Test");
            }

            return RedirectToAction("giris");
        }
    }
}