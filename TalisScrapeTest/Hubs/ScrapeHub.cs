using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TalisScrapeTest.Hubs
{
    public class User
    {
        public string Name { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }

    //[HubName("ScrapeHub")]
    public class ScrapeHub : Hub
    {
        private static readonly ConcurrentDictionary<string, User> Users
    = new ConcurrentDictionary<string, User>(StringComparer.InvariantCultureIgnoreCase);


        public override Task OnConnected()
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new User
            {
                Name = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool _test)
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            User user;
            Users.TryGetValue(userName, out user);

            if (user == null) return base.OnDisconnected(_test);

            lock (user.ConnectionIds)
            {

                user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                if (!user.ConnectionIds.Any())
                {

                    User removedUser;
                    Users.TryRemove(userName, out removedUser);
                }
            }

            return base.OnDisconnected(_test);
        }

        public static User GetUser(string username)
        {

            User user;
            Users.TryGetValue(username, out user);

            return user;
        }

        public void Send(string message, string to)
        {

            User receiver;
            if (Users.TryGetValue(to, out receiver))
            {

                User sender = GetUser(Context.User.Identity.Name);

                IEnumerable<string> allReceivers;
                lock (receiver.ConnectionIds)
                {
                    lock (sender.ConnectionIds)
                    {

                        allReceivers = receiver.ConnectionIds.Concat(sender.ConnectionIds);
                    }
                }

                foreach (var cid in allReceivers)
                {
                    Clients.Client(cid).received(new { sender = sender.Name, message = message, isPrivate = true });
                }
            }
        }




        public IEnumerable<string> GetConnectedUsers()
        {

            return Users.Where(x =>
            {

                lock (x.Value.ConnectionIds)
                {

                    return !x.Value.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
                }

            }).Select(x => x.Key);
        }

        public async Task JoinGroup(string group)
        {
            await Groups.Add(Context.ConnectionId, group);
            Clients.Group(group).doMessage("Joined Group:" + group);
        }

        public async Task LeaveGroup(string group)
        {
            await Groups.Remove(Context.ConnectionId, group);
            Clients.Group(group).doMessage("Left Group:" + group);
        }
    }
}