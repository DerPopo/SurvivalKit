using System;

namespace SurvivalKit.Permissions
{
	/// <summary>
	/// Abstract class for all PermissionManagers.
	/// </summary>
	public abstract class PermissionManager
	{
		private static PermissionManager theManager;
		/// <summary>
		/// Gets / sets the main PermissionManager.
		/// </summary>
		/// <value>The PermissionManager.</value>
		public static PermissionManager Instance {
			get { return theManager; }
			set { theManager = value; }
		}

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="permission">The permission to check.</param>
		/// <param name="isExplicitly">(Out) Set to <c>true</c>, if the permission was set explicitly, <c>false</c> if the permission is set by default (currently only false).</param> 
		public abstract bool isPermitted(CommandSender sender, string permission, out bool isExplicitly);

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="permission">The permission to check.</param>
		/// <param name="isExplicitly">(Out) Set to <c>true</c>, if the permission was set explicitly, <c>false</c> if the permission is set by default (currently only false).</param> 
		public abstract bool isPermitted(string senderId, string permission, out bool isExplicitly);

		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the group has the permission, <c>false</c> otherwise.</returns>
		/// <param name="groupName">The group's name.</param>
		/// <param name="permission">The permission.</param>
		/// <param name="explicitly">(Out) Set to <c>true</c>, if the permission was set explicitly (- before a permission means explicitly disallowed), <c>false</c> otherwise.</param> 
		public abstract bool groupIsPermitted(string groupName, string permission, out bool explicitly);

		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the group has the permission, <c>false</c> otherwise.</returns>
		/// <param name="groupName">The group's name.</param>
		/// <param name="permission">The permission.</param>
		public abstract bool groupIsPermitted(string groupName, string permission);

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="permission">The permission to check.</param>
		public abstract bool isPermitted(CommandSender sender, string permission);

		/// <summary>
		/// Gets whether a CommandSender has a permission.
		/// </summary>
		/// <returns><c>true</c>, if the CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="permission">The permission to check.</param>
		public abstract bool isPermitted(string senderId, string permission);

		/// <summary>
		/// Gets whether the sender has operator privileges.
		/// </summary>
		/// <returns><c>true</c>, if the sender has operator privileges, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		public abstract bool isOp(CommandSender sender);

		/// <summary>
		/// Reloads the permissions.xml .
		/// </summary>
		public abstract void reload();

		/// <summary>
		/// Creates a new permission group.
		/// </summary>
		/// <param name="name">The name of the new group.</param>
		public abstract void createGroup(string name);

		/// <summary>
		/// Removes a group and removes players from the group.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		public abstract bool removeGroup(string name);

		/// <summary>
		/// Adds a permission to a group.
		/// </summary>
		/// <param name="groupName">The group to add the permission to.</param>
		/// <param name="permission">The permission to add.</param>
		public abstract void addPermissionToGroup(string groupName, string permission);
		/// <summary>
		/// Adds a permission to a group.
		/// </summary>
		/// <param name="groupName">The group to add the permission to.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If <c>true</c>, append the permission, otherwise prepend the permission.</param>
		public abstract void addPermissionToGroup(string groupName, string permission, bool append);

		/// <summary>
		/// Removes a permission from a group.
		/// </summary>
		/// <param name="groupName">The name of the group to remove a permission from.</param>
		/// <param name="permission">The permission to remove.</param>
		public abstract void removePermissionFromGroup(string groupName, string permission);

		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		/// <param name="append">If <c>true</c>, append the group import, otherwise prepend it.</param>
		public abstract void addCommandSenderToGroup(CommandSender sender, string groupName, bool append);
		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		/// <param name="append">If <c>true</c>, append the group import, otherwise prepend it.</param>
		public abstract void addCommandSenderToGroup(string senderId, string groupName, bool append);
		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public abstract void addCommandSenderToGroup(CommandSender sender, string groupName);
		/// <summary>
		/// Adds a CommandSender to a group.
		/// </summary>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public abstract void addCommandSenderToGroup(string senderId, string groupName);

		/// <summary>
		/// Removes a CommandSender from a group.
		/// </summary>
		/// <returns><c>true</c>, command sender was removed from the group, <c>false</c> otherwise.</returns>
		/// <param name="sender">The CommandSender.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public abstract bool removeCommandSenderFromGroup(CommandSender sender, string groupName);

		/// <summary>
		/// Removes a CommandSender from a group.
		/// </summary>
		/// <returns><c>true</c>, command sender was removed from the group, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The CommandSender id.</param>
		/// <param name="groupName">The group to add the CommandSender to.</param>
		public abstract bool removeCommandSenderFromGroup(string senderId, string groupName);


		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="sender">The CommandSender to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		public abstract void addPermissionTo(CommandSender sender, string permission);
		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="senderId">The CommandSender id to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		public abstract void addPermissionTo(string senderId, string permission);
		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="sender">The CommandSender to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If <c>true</c>, append the permission, otherwise prepend the permission.</param> 
		public abstract void addPermissionTo(CommandSender sender, string permission, bool append);
		/// <summary>
		/// Adds a permission to a CommandSender
		/// </summary>
		/// <param name="senderId">The CommandSender id to add a permission to.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If <c>true</c>, append the permission, otherwise prepend the permission.</param> 
		public abstract void addPermissionTo(string senderId, string permission, bool append);

		/// <summary>
		/// Removes a permission from a CommandSender
		/// </summary>
		/// <param name="sender">The CommandSender to remove a permission from.</param>
		/// <param name="permission">The permission to remove.</param>
		public abstract void removePermissionFrom(CommandSender sender, string permission);

		/// <summary>
		/// Removes a permission from a CommandSender
		/// </summary>
		/// <param name="senderId">The CommandSender id to remove a permission from.</param>
		/// <param name="permission">The permission to remove.</param>
		public abstract void removePermissionFrom(string senderId, string permission);
	}
}

