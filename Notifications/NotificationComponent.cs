using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNet.SignalR;
using OnlineCoaching.Linq_To_Sql;

namespace OnlineCoaching.Notifications
{
    /*=============================================================
     CreatedBy      :   Dilshad A.
     Created Date   :   07 Sept 2020
     Description    :   To Show Push Notification details
     =============================================================*/
    public class NotificationComponent
    {
        //Here we will add a function for register notification (will add sql dependency)
        public void RegisterNotification(DateTime currentTime)
        {
            string strConStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            string strSqlCommand = @"SELECT [UserType],[ActivityOperation] FROM [dbo].[tblActivity] WHERE [ActivityDate] > @ActivityDate";

            //you can notice here I have added table name like this [dbo].[tblActivity] with [dbo], its mendatory when you use Sql Dependency

            using(SqlConnection con =new SqlConnection(strConStr))
            {
                SqlCommand cmd = new SqlCommand(strSqlCommand, con);
                cmd.Parameters.AddWithValue("ActivityDate", currentTime);
                if(con.State!= ConnectionState.Open)
                {
                    con.Open();
                }

                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += SqlDep_OnChange;

                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now
                }
            }
        }

        private void SqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //or you can also check => if (e.Info == SqlNotificationInfo.Insert) , if you want notification only for inserted record
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= SqlDep_OnChange;

                //from here we will send notification message to client
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");
                //re-register notification
                RegisterNotification(DateTime.Now);
            }
        }

        public List<tblActivity> GetContacts(DateTime afterDate)
        {
            using (CourseDataContext obj = new CourseDataContext())
            {
                return obj.tblActivities.Where(d => d.ActivityDate > afterDate).OrderByDescending(d => d.ActivityDate).ToList();
            }
        }
    }
}