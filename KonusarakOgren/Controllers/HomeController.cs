using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KonusarakOgren.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KonusarakOgren.Controllers
{
    public class HomeController : Controller
    {
        public String html;
        public Uri url;
        
        SQLiteCommand cmd;
        SQLiteConnection baglan;
        string link;
        //Veri tabanımıza bağlanılıyor
        public void sqlBaglan()
        {
            baglan = new SQLiteConnection();
            baglan.ConnectionString = ("Data Source=Models/KonusarakOgren.db");
            baglan.Open();
        }
        //verdiğimiz lik,xpath ve dip ile o yerdeki attiributenin alınması sağlanıyor
        public string linkGetir(string linkUrl,string xPath,string dip)
        {
            url = new Uri(""+linkUrl);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            html = client.DownloadString(url);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            link = doc.DocumentNode.SelectSingleNode(""+xPath).Attributes[dip].Value;
            return link;
        }
        //yönlendirdiğimiz sayfadaki konuyu getirtiyor.
        public string veriGetir(string linkUrl, string linkDevamUrl, string xPath)
        {
            url = new Uri("" + linkUrl+""+linkDevamUrl);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            html = client.DownloadString(url);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            link = doc.DocumentNode.SelectSingleNode("" + xPath).InnerText;
            return link;
        }

        public IActionResult Index()
        {
            List<string> baslik = new List<string>();
          
            baslik.Add(veriGetir("https://www.wired.com/", "", "//*[@id='app-root']/div/div[3]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[1]/div/ul/li[2]/a[2]/h2"));
            baslik.Add(veriGetir("https://www.wired.com/", "", "//*[@id='app-root']/div/div[3]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div/ul/li[2]/a[2]/h2"));
            baslik.Add(veriGetir("https://www.wired.com/", "", "//*[@id='app-root']/div/div[3]/div/div/div[2]/div[1]/div/div[1]/div[2]/div[1]/div[1]/div/ul/li[2]/a[2]/h2"));
            baslik.Add(veriGetir("https://www.wired.com/", "", "//*[@id='app-root']/div/div[3]/div/div/div[2]/div[1]/div/div[1]/div[2]/div[1]/div[2]/div/ul/li[2]/a[2]/h2"));
            baslik.Add(veriGetir("https://www.wired.com/", "", "//*[@id='app-root']/div/div[3]/div/div/div[2]/div[1]/div/div[1]/div[2]/div[2]/div/ul/li[2]/a[2]/h2"));
            ViewBag.baslik1 = baslik[0];
            ViewBag.baslik2 = baslik[1];
            ViewBag.baslik3 = baslik[2];
            ViewBag.baslik4 = baslik[3];
            ViewBag.baslik5 = baslik[4];
          
            


            return View();
        }
        [HttpGet]
        public IActionResult ıcerikleriGetir(string baslik)
        {
              
                string url = linkGetir("https://www.wired.com/", "//*[@id='app-root']"+baslik, "href");
                string veri = veriGetir("https://www.wired.com/", url, "//*[@id='app-root']/div/div[3]/div/div[3]/div[1]/div[2]/main/article/div[1]");
                return Json(veri);
            
         
        }
        [HttpPost]
        public  IActionResult testOlustur(soru soru)
        {
            string veri = veriGetir("https://www.wired.com/", "", "//*[@id='app-root']" + soru.baslik);
            soru.baslik=veri;
            sqlBaglan();
            string sorgu = "insert into text(baslik,icerik) values(@baslik,@icerik)";

            cmd = new SQLiteCommand(sorgu, baglan);
            cmd.Parameters.AddWithValue("@baslik", soru.baslik);
            cmd.Parameters.AddWithValue("@icerik", soru.icerik);
            cmd.ExecuteNonQuery();
            string ıdBul = "select id from text where baslik=@baslik";
            SQLiteCommand cmd1 = new SQLiteCommand(ıdBul, baglan);
            cmd1.Parameters.AddWithValue("@baslik", soru.baslik);
            int id = 0;
            using (var reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                }
            }
            string testOlustur = "insert into question(textId,soru,secenekA,secenekB,secenekC,secenekD,dogruCevap) values(@textId,@soru,@secenekA,@secenekB,@secenekC,@secenekD,@dogruCevap)";

            cmd = new SQLiteCommand(testOlustur, baglan);
            for (int i = 0; i < 4; i++)
            {
                cmd.Parameters.AddWithValue("@textId", id);
                cmd.Parameters.AddWithValue("@soru", soru.sorular[i]);
                cmd.Parameters.AddWithValue("@secenekA", soru.secenekA[i]);
                cmd.Parameters.AddWithValue("@secenekB", soru.secenekB[i]);
                cmd.Parameters.AddWithValue("@secenekC", soru.secenekC[i]);
                cmd.Parameters.AddWithValue("@secenekD", soru.secenekD[i]);
                if (soru.dogruCevap[i]=="A")
                {
                    cmd.Parameters.AddWithValue("@dogruCevap", soru.secenekA[i]);
                }
                else if (soru.dogruCevap[i] == "B")
                {
                    cmd.Parameters.AddWithValue("@dogruCevap", soru.secenekB[i]);
                }
             else if (soru.dogruCevap[i] == "C")
                {
                    cmd.Parameters.AddWithValue("@dogruCevap", soru.secenekC[i]);
                }
               else if (soru.dogruCevap[i] == "D")
                {
                    cmd.Parameters.AddWithValue("@dogruCevap", soru.secenekD[i]);
                }
                cmd.ExecuteNonQuery();
            }






            return RedirectToAction("testleriGetir", "test");
        }
       
    }
}