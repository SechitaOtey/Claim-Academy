using PWND2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWND2
{
    public class Utilities
    {
        public static void calculateChallengeWinners(PWNDEntities db)
        {
            var finishedChallenges = db.Challenges.Where(c => c.EndDate < DateTime.Now && c.WinningActivityLog == null);
            foreach (var challenge in finishedChallenges)
            {
                challenge.WinningActivityLog=challenge.ActivityLogs.OrderByDescending(al => al.Points).FirstOrDefault();
                db.Entry(challenge).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();

        }
    }
}