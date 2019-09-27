using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PWND2.Models;

namespace PWND2.Controllers
{   [Authorize]
    public class KIDsController : Controller
    {
        private PWNDEntities db = new PWNDEntities();

        // GET: KIDs
        
        // GET: KIDs/Details/5
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KID kID = db.KIDs.Find(id);
            if (kID == null)
            {
                return HttpNotFound();
            }
            return View(kID);
        }



        // GET: KIDs/Edit/5
        public ActionResult Edit()
        {
            var id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KID kID = db.KIDs.Find(id);
            if (kID == null)
            {
                return HttpNotFound();
            }
            return View(kID);
        }

        // POST: KIDs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KidName,Email,PointsGoal")] KID kID)
        {
            if (ModelState.IsValid)
            {
                kID.KidiD = User.Identity.GetUserId();
                db.Entry(kID).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kID);
        }  



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
