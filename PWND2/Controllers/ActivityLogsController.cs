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
{
    [Authorize]
    public class ActivityLogsController : Controller
    {
        private PWNDEntities db = new PWNDEntities();

        // GET: ActivityLogs
        public ActionResult Index()
        {
            Utilities.calculateChallengeWinners(db);
            var userId = User.Identity.GetUserId();
            var kid = db.KIDs.Find(userId);
            var unorderedActivityLogs = kid.ActivityLogs;
            var activityLogs = unorderedActivityLogs.OrderByDescending(al=>al.LogDate);
            var Now = DateTime.Today;
            var DayOfWeek = (int)Now.DayOfWeek;
            var StartofWeek = Now.AddDays(-DayOfWeek);
            var WeeklyPoints = activityLogs.Where(al => al.LogDate >= StartofWeek).Sum(al => al.Points);
            ViewBag.WeeklyPoints = WeeklyPoints;
            ViewBag.PointsGoal = kid.PointsGoal;


            return View(activityLogs.ToList());
        }

        // GET: ActivityLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityLog activityLog = db.ActivityLogs.Find(id);
            if (activityLog == null)
            {
                return HttpNotFound();
            }
            return View(activityLog);
        }

        // GET: ActivityLogs/Create
        public ActionResult Create()
        {
            var model = new ActivityLog()
            {
                KidiD = User.Identity.GetUserId(),
                LogDate = DateTime.Now

            };

            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "ActivityName");
            
            return View(model);
        }

        // POST: ActivityLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "ActivityId,RawScore,LogDate,ChallengeId")] ActivityLog activityLog)
        {
            if (ModelState.IsValid)
            {
                var activities = db.Activities.Find(activityLog.ActivityId);
                activityLog.Points = activityLog.RawScore * activities.PointMultiplier;
                activityLog.KidiD = User.Identity.GetUserId();
                db.ActivityLogs.Add(activityLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "ActivityName", activityLog.ActivityId);
            ViewBag.ChallengeId = new SelectList(db.Challenges, "ChallengeId", "ChallengeId", activityLog.ChallengeId);
            ViewBag.KidiD = new SelectList(db.KIDs, "KidiD", "KidName", activityLog.KidiD);
            return View(activityLog);
        }

        // GET: ActivityLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityLog activityLog = db.ActivityLogs.Find(id);
            if (activityLog == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "ActivityName", activityLog.ActivityId);
            ViewBag.ChallengeId = new SelectList(db.Challenges, "ChallengeId", "ChallengeId", activityLog.ChallengeId);
            ViewBag.KidiD = new SelectList(db.KIDs, "KidiD", "KidName", activityLog.KidiD);
            return View(activityLog);
        }

        // POST: ActivityLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivtyLogId,KidiD,ActivityId,RawScore,LogDate,Points,ChallengeId")] ActivityLog activityLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activityLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "ActivityName", activityLog.ActivityId);
            ViewBag.ChallengeId = new SelectList(db.Challenges, "ChallengeId", "ChallengeId", activityLog.ChallengeId);
            ViewBag.KidiD = new SelectList(db.KIDs, "KidiD", "KidName", activityLog.KidiD);
            return View(activityLog);
        }

        // GET: ActivityLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityLog activityLog = db.ActivityLogs.Find(id);
            if (activityLog == null)
            {
                return HttpNotFound();
            }
            return View(activityLog);
        }

        // POST: ActivityLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActivityLog activityLog = db.ActivityLogs.Find(id);
            db.ActivityLogs.Remove(activityLog);
            db.SaveChanges();
            return RedirectToAction("Index");
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
