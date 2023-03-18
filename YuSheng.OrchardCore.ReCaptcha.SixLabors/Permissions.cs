using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission SixLaborsCaptcha
                 = new Permission(nameof(SixLaborsCaptcha), "Manage SixLaborsCaptcha settings");



        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[]
            {
                SixLaborsCaptcha
            }
            .AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            yield return new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[]
                {
                    SixLaborsCaptcha
                }
            };
        }
    }
}
