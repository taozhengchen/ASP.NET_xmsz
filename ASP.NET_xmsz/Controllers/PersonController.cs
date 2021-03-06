﻿using Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASP.NET_xmsz.Models;
using PagedList;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;
using System.IO;
using Biz.Entity;

namespace ASP.NET_xmsz.Controllers
{
    public class PersonController : Controller
    {
        // GET: person
        public ActionResult Index(int postpage = 1, int replypage = 1, int id = 0)
        {
            if (id == 0 && Session["userId"] != null)
            {
                id = (int)Session["userId"];
                ViewBag.own = "true";

            }
            else
            {
                ViewBag.own = "false";
            }
            if (id == 0)
            {
                return View("../Home/Index");
            }
            Person per = new Person();
            per.user = new GetUsers().SelectUser(id);
            per.post = new GetPost().showListByUser(id).OrderByDescending(p => p.CreateTime).ToPagedList(postpage, 5);
            per.reply = new GetReply().showListByUser(id).OrderByDescending(p => p.ReplyTime).ToPagedList(replypage, 5);
            return View("personIndex", per);
        }
        public ActionResult Edit()
        {
            int id = (int)Session["userId"];
            Personedit pr = new Personedit();
            GetDistricts gd = new GetDistricts();
            pr.user = new GetUsers().SelectUser(id);
            District d = gd.getProvince(pr.user.districtId);
            if (d.parent_id == 0)
            {
                pr.parent = d.id;
            }
            else
            {
                pr.parent = d.parent_id;
            }
            pr.city = d.id;
            return View("PersonEdit", pr);
        }
        [HttpPost]
        public ActionResult DoEdit(Users us)
        {
            int id = (int)Session["userId"];
            GetUsers gu = new GetUsers();
            Users up = gu.SelectUser(id);
            up.sex = us.sex;
            up.districtId = us.districtId;
            up.introduce = us.introduce;
            up.birthday = us.birthday;
            gu.UpdateUser(up);
            return RedirectToAction("../Person/Index");
        }
        [HttpPost]
        public string GetProvince()
        {
            GetDistricts bll = new GetDistricts();
            IList<District> list = bll.selectProvince();
            DataContractJsonSerializer json = new DataContractJsonSerializer(list.GetType());
            string szJson = "";
            using (MemoryStream stream = new MemoryStream())
            {

                json.WriteObject(stream, list);
                szJson = Encoding.UTF8.GetString(stream.ToArray());
            };
            return szJson;
        }
        [HttpPost]
        public string GetCity(string pro)
        {
            GetDistricts bll = new GetDistricts();
            IList<District> list = bll.selectCity(int.Parse(pro));
            DataContractJsonSerializer json = new DataContractJsonSerializer(list.GetType());
            string szJson = "";
            using (MemoryStream stream = new MemoryStream())
            {

                json.WriteObject(stream, list);
                szJson = Encoding.UTF8.GetString(stream.ToArray());
            };
            return szJson;
        }
    }
}