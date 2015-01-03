using System;
using System.IO;

namespace SurvivalKit.Permissions
{
	/// <summary>
	/// The default PermissionManager that uses ./config/permissions.xml as permissions file.
	/// </summary>
	public class SimplePermissionManager : PermissionManager
	{
		private PermissionFile permissionFile;
		/// <summary>
		/// Initializes a new SimplePermissionManager and parses the ./config/permissions.xml file.
		/// </summary>
		public SimplePermissionManager()
		{
			DirectoryInfo configDir = new System.IO.DirectoryInfo("config");
			if (!configDir.Exists)
				configDir.Create();
			FileInfo permissionsXml = new FileInfo("./config/permissions.xml");
			if (!permissionsXml.Exists)
				permissionsXml.Create();
			this.permissionFile = new PermissionFile("./config/permissions.xml");
		}

		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		/// <param name="append">If <c>true</c>, append the group import, otherwise prepend it.</param>
		public override void addCommandSenderToGroup(CommandSender sender, string groupName, bool append)
		{
			this.permissionFile.addSenderToGroup(sender.DefiniteName, groupName, append);
		}
		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		/// <param name="append">If <c>true</c>, append the group import, otherwise prepend it.</param>
		public override void addCommandSenderToGroup(string senderId, string groupName, bool append)
		{
			string definiteName = PlayerCommandSender.MakeDefiniteName(senderId);
			if (definiteName == null)
				return;
			this.permissionFile.addSenderToGroup(definiteName, groupName, append);
		}
		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public override void addCommandSenderToGroup(CommandSender sender, string groupName)
		{
			this.permissionFile.addSenderToGroup(sender.DefiniteName, groupName);
		}
		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public override void addCommandSenderToGroup(string senderId, string groupName)
		{
			this.permissionFile.addSenderToGroup(senderId, groupName, true);
		}

		/// <summary>
		/// Removes a CommandSender from a group.
		/// </summary>
		/// <returns><c>true</c>, command sender was removed from the group, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public override bool removeCommandSenderFromGroup(CommandSender sender, string groupName)
		{
			return this.permissionFile.removeSenderFromGroup(sender.DefiniteName, groupName);
		}

		/// <summary>
		/// Removes a CommandSender from a group.
		/// </summary>
		/// <returns><c>true</c>, command sender was removed from the group, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public override bool removeCommandSenderFromGroup(string senderId, string groupName)
		{
			string definiteName = PlayerCommandSender.MakeDefiniteName(senderId);
			if (definiteName == null)
				return false;
			return this.permissionFile.removeSenderFromGroup(definiteName, groupName);
		}

		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="sender">The CommandSender to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		public override void addPermissionTo(CommandSender sender, string permission)
		{
			this.permissionFile.addPermissionToSender(sender.DefiniteName, permission);
		}
		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="senderId">The CommandSender id to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		public override void addPermissionTo(string senderId, string permission)
		{
			string definiteName = PlayerCommandSender.MakeDefiniteName (senderId);
			if (definiteName == null)
				return;
			this.permissionFile.addPermissionToSender (definiteName, permission);
		}
		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="sender">The CommandSender to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If <c>true</c>, append the permission, otherwise prepend the permission.</param> 
		public override void addPermissionTo(CommandSender sender, string permission, bool append)
		{
			this.permissionFile.addPermissionToSender(sender.DefiniteName, permission, append);
		}
		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="senderId">The CommandSender id to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If <c>true</c>, append the permission, otherwise prepend the permission.</param> 
		public override void addPermissionTo(string senderId, string permission, bool append)
		{
			string definiteName = PlayerCommandSender.MakeDefiniteName(senderId);
			if (definiteName == null)
				return;
			this.permissionFile.addPermissionToSender(definiteName, permission, append);
		}

		/// <summary>
		/// Adds a permission to a group.
		/// </summary>
		/// <param name="groupName">The group to add the permission to.</param>
		/// <param name="permission">The permission to add.</param>
		public override void addPermissionToGroup (string groupName, string permission)
		{
			this.permissionFile.addPermissionToGroup(groupName, permission);
		}

		/// <summary>
		/// Adds a permission to a group.
		/// </summary>
		/// <param name="groupName">The group to add the permission to.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If set to <c>true</c> append.</param>
		public override void addPermissionToGroup (string groupName, string permission, bool append)
		{
			this.permissionFile.addPermissionToGroup(groupName, permission, append);
		}

		/// <summary>
		/// Creates a new permission group.
		/// </summary>
		/// <param name="name">The name of the new group.</param>
		public override void createGroup(string name)
		{
			this.permissionFile.appendGroup(name);
		}

		/// <summary>
		/// Removes a group and removes players from the group.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		public override bool removeGroup (string name)
		{
			return this.permissionFile.removeGroup(name);
		}

		/// <summary>
		/// Gets whether the sender has operator privileges.
		/// </summary>
		/// <returns><c>true</c>, if the sender has operator privileges, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		public override bool isOp(CommandSender sender)
		{
			return permissionFile.senderIsAllowedTo(sender.DefiniteName, "*");//sender.isOp();
		}

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="permission">The permission to check.</param>
		/// <param name="isExplicitly">(Out) Set to <c>true</c>, if the permission was set explicitly, <c>false</c> if the permission is set by default (currently only false).</param> 
		public override bool isPermitted(CommandSender sender, string permission, out bool isExplicitly)
		{
			return isPermitted(sender.DefiniteName, permission, out isExplicitly);
		}

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="permission">The permission to check.</param>
		/// <param name="isExplicitly">(Out) Set to <c>true</c>, if the permission was set explicitly, <c>false</c> if the permission is set by default (currently only false).</param> 
		public override bool isPermitted(string senderId, string permission, out bool isExplicitly)
		{
			string definiteName = PlayerCommandSender.MakeDefiniteName(senderId);
			if (definiteName == null)
			{
				isExplicitly = false;
				return false;
			}
			return this.permissionFile.senderIsAllowedTo(definiteName, permission, out isExplicitly);
		}

		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the group has the permission, <c>false</c> otherwise.</returns>
		/// <param name="groupName">The group's name.</param>
		/// <param name="permission">The permission.</param>
		/// <param name="explicitly">(Out) Set to <c>true</c>, if the permission was set explicitly (- before a permission means explicitly disallowed), <c>false</c> otherwise.</param> 
		public override bool groupIsPermitted(string groupName, string permission, out bool explicitly)
		{
			return this.permissionFile.groupIsAllowedTo(groupName, permission, out explicitly);
		}

		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the group has the permission, <c>false</c> otherwise.</returns>
		/// <param name="groupName">The group's name.</param>
		/// <param name="permission">The permission.</param>
		public override bool groupIsPermitted(string groupName, string permission)
		{
			return this.permissionFile.groupIsAllowedTo(groupName, permission);
		}

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="permission">The permission to check.</param>
		public override bool isPermitted(CommandSender sender, string permission)
		{
			return isPermitted(sender.DefiniteName, permission);
			//return this.permissionFile.senderIsAllowedTo(sender.DefiniteName, permission);
		}

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="permission">The permission to check.</param>
		public override bool isPermitted(string senderId, string permission)
		{
			/*string[] permissionSub = permission.Split(new string[]{ "." }, StringSplitOptions.None);
			string[] permissions = new string[permissionSub.Length + 1];

			for (int i = 0; i < permissionSub.Length; i++) {
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				for (int i_ = 0; i_ < i; i_++) {
					builder.Append(permissionSub[i_]);
					builder.Append(".");
				}
				builder.Append("*");
				permissions[i] = builder.ToString();
			}
			permissions[permissions.Length - 1] = permission;*/

			string definiteName = PlayerCommandSender.MakeDefiniteName(senderId);
			if (definiteName == null)
				return false;
			return this.permissionFile.senderIsAllowedTo(definiteName, permission);
			/*foreach (string curPermission in permissions)
			{
				if (this.permissionFile.senderIsAllowedTo(definiteName, curPermission))
					return true;
			}
			return false;*/
		}

		/// <summary>
		/// Removes a permission from a CommandSender
		/// </summary>
		/// <param name="sender">The CommandSender to remove a permission from.</param>
		/// <param name="permission">The permission to remove.</param>
		public override void removePermissionFrom(CommandSender sender, string permission)
		{
			this.permissionFile.removePermissionFromSender(sender.DefiniteName, permission);
		}

		/// <summary>
		/// Removes a permission from a CommandSender
		/// </summary>
		/// <param name="senderId">The CommandSender id to remove a permission from.</param>
		/// <param name="permission">The permission to remove.</param>
		public override void removePermissionFrom(string senderId, string permission)
		{
			string definiteName = PlayerCommandSender.MakeDefiniteName(senderId);
			if (definiteName == null)
				return;
			this.permissionFile.removePermissionFromSender(definiteName, permission);
		}

		/// <summary>
		/// Removes a permission from a group.
		/// </summary>
		/// <param name="groupName">The name of the group to remove a permission from.</param>
		/// <param name="permission">The permission to remove.</param>
		public override void removePermissionFromGroup(string groupName, string permission)
		{
			this.permissionFile.removePermissionFromGroup(groupName, permission);
		}

		/// <summary>
		/// Reloads the permissions.xml .
		/// </summary>
		public override void reload()
		{
			this.permissionFile.reload();
		}
	}
}

