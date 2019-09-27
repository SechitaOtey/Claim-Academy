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
    public class ChallengesController : Controller
    {
        private PWNDEntities db = new PWNDEntities();

        // GET: Challenges
       
        public ActionResult Index(string id)
        {
            Utilities.calculateChallengeWinners(db);

            var challenges = db.Challenges.Include(c => c.Activity).Include(c => c.WinningActivityLog);
            if ("Past".Equals(id, StringComparison.InvariantCultureIgnoreCase))
            {
                challenges = challenges.Where(c => c.EndDate < DateTime.Now);
            }
            else
            {
                challenges = challenges.Where(c => c.EndDate > DateTime.Now);
            }
            return View(challenges.ToList());
        }

        // GET: Challenges/Details/5
       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Challenge challenge = db.Challenges.Find(id);
            if (challenge == null)
            {
                return HttpNotFound();
            }
            return View(challenge);
        }

        // GET: Challenges/Create
        public ActionResult Create()
        {
            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "ActivityName");
            ViewBag.WinnerId = new SelectList(db.ActivityLogs, "ActivtyLogId", "KidiD");
            return View();
        }

        // POST: Challenges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChallengeId,Duration,EndDate,ActivityId")] Challenge challenge)
        {
            if (challenge.EndDate > DateTime.Now.AddDays(7)||challenge.EndDate < DateTime.Now.AddMinutes(1)) 
            {
                ModelState.AddModelError("EndDate", "Time Has To Be Between Now & 7 Days");
            }
            if (ModelState.IsValid)
            {
                db.Challenges.Add(challenge);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "ActivityName", challenge.ActivityId);
            ViewBag.WinnerId = new SelectList(db.ActivityLogs, "ActivtyLogId", "KidiD", challenge.WinnerId);
            return View(challenge);
        }

        public ActionResult EnteredChallenge(int? id)

        {
            var challenge = db.Challenges.Find(id);
            if(challenge==null)
            {
                return new HttpNotFoundResult();
            }
            var model = new ActivityLog()
            {
                KidiD = User.Identity.GetUserId(),
                ActivityId=challenge.ActivityId,
                Activity =challenge.Activity,
                ChallengeId=id,
                Challenge=challenge,
                LogDate=DateTime.Now

            };
            
            return View(model);
        }

        public ActionResult SendChallenge(int? id)
        {
            var model = new SendChallengeVM { challengeId = id.Value };
            var allKIds = db.KIDs.ToList();
            ViewBag.AllKids = new MultiSelectList(allKIds, "KidiD", "KidName");
            return View(model);
        }
        [HttpPost]
        public ActionResult SendChallenge(int? id, SendChallengeVM model)
        {
            if (ModelState.IsValid)
            {
                var challengerUserId = User.Identity.GetUserId();
                var challenger = db.KIDs.Find(challengerUserId);
                var challenge = db.Challenges.Find(id);
                foreach (var kidid in model.KIdIds)
                {
                    var kid = db.KIDs.Find(kidid);
                    var challengeUrl = Url.Action("EnteredChallenge","Challenges",new { id = challenge.ChallengeId }, protocol:Request.Url.Scheme) ;
                    var message = $@"{challenger.KidName} has challenged you!
 
The challenge is to do {challenge.Activity.ActivityName}, which will test your {challenge.Activity.ActivityCategory.Category}. 
How many {challenge.Activity.ScoringFactor} can you do in {challenge.Duration}?

When have completed the challenge click here: {challengeUrl}";
                    MessageSending.MessageSender.SendEmail(kid.Email, "Challenge", message);
                    return RedirectToAction("Index");

                }
            }
            
             model.challengeId = id.Value ;
            var allKIds = db.KIDs.ToList();
            ViewBag.AllKids = new MultiSelectList(allKIds, "KidiD", "KidName");
            return View(model);
        }

        //// GET: Challenges/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Challenge challenge = db.Challenges.Find(id);
        //    if (challenge == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(challenge);
        //}

        //// POST: Challenges/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Challenge challenge = db.Challenges.Find(id);
        //    db.Challenges.Remove(challenge);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
