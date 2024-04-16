using BetaCycleAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using EncryptData;
using BetaCycleAPI.Models.Enums;

namespace BetaCycleAPI.BLogic.Authentication
{
    public class CredentialsDBChecker
    {
        private readonly AdventureWorksLt2019Context _context;
        private readonly AdventureWorks2019CredentialsContext _credentials;

        public CredentialsDBChecker(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        public CredentialsDBChecker CreateChecker()
        {
            return new CredentialsDBChecker(_context, _credentials);
        }
        public async Task<DBCheckResponse> ValidateLoginAsync(string user, string pwd)
        {
            DBCheckResponse response = DBCheckResponse.NotFound;
            try
            {
                var credLoginNew = await _credentials.Credentials.Where(c => c.Email == user).ToListAsync();
                var credLoginOld = await _context.Customers.Where(c => c.EmailAddress == user).ToListAsync();
                if (credLoginNew.Any()) //new db
                {
                    if ((EncryptData.CypherData.DecryptSalt(pwd, credLoginNew.First().SaltHash)).Equals(credLoginNew.First().PasswordHash))
                    {
                        response = DBCheckResponse.FoundMigrated;
                    }
                }
                else if (credLoginOld.Any()) //old db
                {
                    response = DBCheckResponse.FoundNotMigrated;
                }
            }catch(Exception ex)
            {
                throw;
            }
            return response;
        }
    }
}
