using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ActiveDirectory
{
    public class ActiveDirectoryManager
    {

        private DirectoryEntry _directoryEntry = null;
        private string LDAPPath { get; set; }
        private string LDAPUser { get; set; }
        private string LDAPPassword { get; set; }
        private string LDAPDomain { get; set; }

        public ActiveDirectoryManager(string path = null, string user = null, string password = null, string domain = null)
        {
            LDAPPath = String.IsNullOrEmpty(path) ? null : path;
            LDAPUser = String.IsNullOrEmpty(user) ? null : user;
            LDAPPassword = String.IsNullOrEmpty(password) ? null : password;
            LDAPDomain = String.IsNullOrEmpty(domain) ? null : domain;
        }
        private DirectoryEntry SearchRoot
        {
            get
            {
                if (_directoryEntry == null)
                {
                    _directoryEntry = new DirectoryEntry(LDAPPath, LDAPUser, LDAPPassword, AuthenticationTypes.Secure);
                }
                return _directoryEntry;
            }
        }

        public List<ADUserDetail> GetUsers()
        {
            var users = new List<ADUserDetail>();
            var rootEntry = new DirectoryEntry("LDAP://OU=O365,OU=Ionia Users,DC=ionia,DC=gr", null, null, AuthenticationTypes.Secure);
            foreach (DirectoryEntry entry in rootEntry.Children)
            {
                var userEntry = new DirectoryEntry(entry.Path, null, null, AuthenticationTypes.Secure);
                var user = ADUserDetail.GetUser(userEntry);
                if (
                    !string.IsNullOrEmpty(user.FirstName) &&
                    !string.IsNullOrEmpty(user.LastName) &&
                    !string.IsNullOrEmpty(user.EmailAddress)
                    )
                {
                    users.Add(user);
                }
            }
            return users;
        }

        public bool canBind(string path, string username, string password)
        {

            DirectoryEntry entry = new DirectoryEntry(path, username, password);

            try
            {
                object nativeObject = entry.NativeObject;
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        internal ADUserDetail GetUserByFullName(String userName)
        {
            try
            {
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Filter = "(&(objectClass=user)(cn=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path, LDAPUser, LDAPPassword);
                    return ADUserDetail.GetUser(user);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal ADUserDetail GetUserByEmail(string email)
        {
            try
            {
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);

                directorySearch.Filter = "(mail=" + email + ")";
                directorySearch.PropertiesToLoad.Add("mail");
                SearchResult result = directorySearch.FindOne();

                if (result != null)
                {
                    DirectoryEntry user = new DirectoryEntry(result.Path, LDAPUser, LDAPPassword);
                    return ADUserDetail.GetUser(user);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ADUserDetail GetUserByLoginName(String userName)
        {
            try
            {
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);

                directorySearch.Filter = "(&(objectClass=user)(SAMAccountName=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path, LDAPUser, LDAPPassword);
                    return ADUserDetail.GetUser(user);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ADUserDetail> GetUserFromGroup(String groupName)
        {
            List<ADUserDetail> userlist = new List<ADUserDetail>();
            try
            {
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + groupName + "))";
                SearchResult results = directorySearch.FindOne();
                if (results != null)
                {
                    DirectoryEntry deGroup = new DirectoryEntry(results.Path, LDAPUser, LDAPPassword);
                    System.DirectoryServices.PropertyCollection pColl = deGroup.Properties;
                    int count = pColl["member"].Count;

                    for (int i = 0; i < count; i++)
                    {
                        string respath = results.Path;
                        string[] pathnavigate = respath.Split("CN".ToCharArray());
                        respath = pathnavigate[0];
                        string objpath = pColl["member"][i].ToString();
                        string path = respath + objpath;

                        DirectoryEntry user = new DirectoryEntry(path, LDAPUser, LDAPPassword);
                        ADUserDetail userobj = ADUserDetail.GetUser(user);
                        userlist.Add(userobj);
                        user.Close();
                    }
                }
                return userlist;
            }
            catch (Exception)
            {
                return userlist;
            }

        }

        #region Get user with First Name

        public List<ADUserDetail> GetUsersByFirstName(string fName)
        {
            //UserProfile user;
            List<ADUserDetail> userlist = new List<ADUserDetail>();
            string filter = "";

            _directoryEntry = null;
            DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
            directorySearch.Asynchronous = true;
            directorySearch.CacheResults = true;
            filter = string.Format("(givenName={0}*", fName);
            //            filter = "(&(objectClass=user)(objectCategory=person)(givenName="+fName+ "*))";

            directorySearch.Filter = filter;

            SearchResultCollection userCollection = directorySearch.FindAll();
            foreach (SearchResult users in userCollection)
            {
                DirectoryEntry userEntry = new DirectoryEntry(users.Path, LDAPUser, LDAPPassword);
                ADUserDetail userInfo = ADUserDetail.GetUser(userEntry);

                userlist.Add(userInfo);
            }

            directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + fName + "*))";
            SearchResultCollection results = directorySearch.FindAll();
            if (results != null)
            {
                foreach (SearchResult r in results)
                {
                    DirectoryEntry deGroup = new DirectoryEntry(r.Path, LDAPUser, LDAPPassword);

                    ADUserDetail agroup = ADUserDetail.GetUser(deGroup);
                    userlist.Add(agroup);
                }
            }
            return userlist;
        }

        #endregion


        public List<GroupPrincipal> GetGroups(string userName)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();

            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);

            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userName);

            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                foreach (Principal p in groups)
                {
                    if (p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                    }
                }
            }
            return result;
        }



        #region AddUserToGroup
        public bool AddUserToGroup(string userlogin, string groupName)
        {
            try
            {
                _directoryEntry = null;
                ADManager admanager = new ADManager(LDAPDomain, LDAPUser, LDAPPassword);
                admanager.AddUserToGroup(userlogin, groupName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region RemoveUserToGroup
        public bool RemoveUserToGroup(string userlogin, string groupName)
        {
            try
            {
                _directoryEntry = null;
                ADManager admanager = new ADManager("xxx", LDAPUser, LDAPPassword);
                admanager.RemoveUserFromGroup(userlogin, groupName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
