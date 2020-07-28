using OpenMod.API.Permissions;
using OpenMod.Core.Permissions;
using OpenMod.Core.Permissions.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PermissionExtensions.Models
{
    public class PermissionRoleExtension : IPermissionRole
    {
        public PermissionRoleExtension()
        {
            Parents = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public PermissionRoleExtension(PermissionRoleData data)
        {
            Id = data.Id;
            Priority = data.Priority;
            DisplayName = data.DisplayName;
            Parents = data.Parents;
            IsAutoAssigned = data.IsAutoAssigned;
            Permissions = data.Permissions;
        }

        public static implicit operator PermissionRole(PermissionRoleExtension role)
        {
            return new PermissionRole(new PermissionRoleData()
            {
                Id = role.Id,
                Priority = role.Priority,
                DisplayName = role.DisplayName,
                IsAutoAssigned = role.IsAutoAssigned,
                Parents = role.Parents,
                Permissions = role.Permissions
            });
        }

        public string Id { get; }
        public int Priority { get; set; }
        public string DisplayName { get; }
        public HashSet<string> Parents { get; }
        public HashSet<string> Permissions { get; }
        public bool IsAutoAssigned { get; set; }
        public string Type { get; } = "role";
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public Color Color { get; set; }
    }
}
