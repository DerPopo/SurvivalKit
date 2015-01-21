using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace SurvivalKit.Permissions
{
	/// <summary>
	/// Represents a permission xml file
	/// </summary>
	public class PermissionFile
	{
		private XmlDocument document;
		private XmlNode rootNode;
		private XmlNode groupsNode;
		private XmlNode membersNode;

		private Dictionary<string,List<string>> permissionsBySender = new Dictionary<string, List<string>>();
		private Dictionary<string,List<string>> permissionsByGroup = new Dictionary<string,List<string>>();

		/// <summary>
		/// Gets a Dictionary containing all permissions by group.
		/// </summary>
		/// <value>The System.Collections.Generic.Dictionary.</value>
		public Dictionary<string,List<string>> PermissionsBySender { get { return new Dictionary<string,List<string>>(this.permissionsBySender); } }

		/// <summary>
		/// Gets a Dictionary containing all permissions by sender.
		/// </summary>
		/// <value>The System.Collections.Generic.Dictionary.</value>
		public Dictionary<string,List<string>> PermissionsByGroup { get { return new Dictionary<string,List<string>>(this.permissionsByGroup); } }

		private string xmlPath;

		private void reload(XmlDocument xmlDoc)
		{
			this.permissionsByGroup.Clear();
			this.permissionsBySender.Clear();
			this.readGroups();
			this.readMembers();
		}

		/// <summary>
		/// Reloads the permissions.xml file.
		/// </summary>
		public void reload()
		{
			document = new XmlDocument();
			try {
				document.Load(xmlPath);
			} catch (XmlException){
				XmlElement rootElem;
				document = new XmlDocument();
				document.AppendChild(document.CreateXmlDeclaration ("1.0", "UTF-8", null));
				document.AppendChild((rootElem = document.CreateElement("permissions")));
				rootElem.AppendChild(document.CreateElement("groups"));
				rootElem.AppendChild(document.CreateElement("members"));
			}
			if (document.SelectSingleNode ("/permissions[1]") == null) {
				document = new XmlDocument();
				XmlDeclaration decl = document.CreateXmlDeclaration ("1.0", "utf-8", null);
				rootNode = document.CreateElement ("permissions");
				document.InsertBefore (decl, document.DocumentElement);
				document.AppendChild (rootNode);

				groupsNode = document.CreateElement ("groups");
				rootNode.AppendChild (groupsNode);
				membersNode = document.CreateElement ("members");
				rootNode.AppendChild (membersNode);
			} else {
				rootNode = document.SelectSingleNode("/permissions[1]");
				groupsNode = document.SelectSingleNode("/permissions/groups[1]");
				membersNode = document.SelectSingleNode("/permissions/members[1]");
				if (groupsNode == null) {
					groupsNode = document.CreateElement("groups");
					rootNode.AppendChild(groupsNode);
				} else if (groupsNode.HasChildNodes) {
					//this.readGroups();
				}
				if (membersNode == null) {
					membersNode = document.CreateElement("members");
					rootNode.AppendChild(membersNode);
				} else if (membersNode.HasChildNodes) {
					//this.readMembers();
				}
			}
			this.reload(document);
			document.Save(xmlPath);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Permissions.PermissionFile"/> class.
		/// </summary>
		/// <param name="path">The path to a permission .xml file.</param> 
		public PermissionFile(string path)
		{
			xmlPath = path;
			this.reload();
		}

		/// <summary>
		/// Adds a permission group.
		/// </summary>
		/// <param name="name">The name of the new group.</param>
		public void appendGroup(string name)
		{
			foreach (string curGroupName in this.permissionsByGroup.Keys)
			{
				if (name.ToLower().Equals(curGroupName.ToLower()))
					return;
			}
			XmlElement newGroup = document.CreateElement("group");
			newGroup.SetAttribute("name", name);
			groupsNode.AppendChild(newGroup);
			this.document.Save(this.xmlPath);
			this.permissionsByGroup.Add(name, new List<string>());
			this.document.Save(this.xmlPath);
		}

		/// <summary>
		/// Removes a group.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		public bool removeGroup(string name)
		{
			foreach (string curGroupName in this.permissionsByGroup.Keys)
			{
				if (curGroupName.ToLower().Equals(name.ToLower()))
				{
					this.permissionsByGroup.Remove(curGroupName);
					List<XmlNode> nodesToRemove = new List<XmlNode>();
					foreach (XmlNode curMemberNode in membersNode.ChildNodes)
					{
						if (curMemberNode.LocalName.ToLower().Equals("member"))
						{
							foreach (XmlNode curSubNode in curMemberNode.ChildNodes)
							{
								string innerText = getNodeVal(curSubNode);
								if (curSubNode.LocalName.ToLower().Equals("fromgroup") && (innerText == null || innerText.ToLower().Equals(curGroupName.ToLower())))
								{
									nodesToRemove.Add(curSubNode);
								}
							}
							foreach (XmlNode curNode in nodesToRemove)
							{
								curMemberNode.RemoveChild(curNode);
							}
							nodesToRemove.Clear();
						}
					}
					foreach (XmlNode curGroupNode in groupsNode.ChildNodes)
					{
						if (curGroupNode.LocalName.ToLower().Equals("group"))
						{
							foreach (XmlNode curSubNode in curGroupNode.ChildNodes)
							{
								string innerText = getNodeVal(curSubNode);
								if (curSubNode.LocalName.ToLower().Equals("fromgroup") && (innerText == null || innerText.ToLower().Equals(curGroupName.ToLower())))
								{
									nodesToRemove.Add(curSubNode);
								}
							}
							foreach (XmlNode curNode in nodesToRemove)
							{
								curGroupNode.RemoveChild(curNode);
							}
							nodesToRemove.Clear();
						}
					}
					foreach (XmlNode curGroupNode in groupsNode.ChildNodes)
					{
						string _curGroupName = getGroupName(curGroupNode);
						if (_curGroupName != null && _curGroupName.ToLower().Equals(curGroupName.ToLower()))
						{
							nodesToRemove.Add(curGroupNode);
						}
					}
					foreach (XmlNode curNode in nodesToRemove)
					{
						groupsNode.RemoveChild(curNode);
					}
					this.document.Save(this.xmlPath);
					this.reload(document);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Adds a permission to a group.
		/// </summary>
		/// <param name="groupName">The name of the group.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If <c>true</c>, append the permission, otherwise prepend the permission.</param> 
		public void addPermissionToGroup(string groupName, string permission, bool append)
		{
			if (permission.Length <= 0 || (permission.Length == 1 && permission[0] == '-'))
				throw new ArgumentException("Permission invalid!");
			this.addPermissionToDictionary(this.permissionsByGroup, groupName, permission, append ? -1 : 0);
			XmlNode groupNode = null;
			foreach (XmlNode curGroupNode in groupsNode.ChildNodes)
			{
				if (curGroupNode.LocalName.ToLower().Equals("group"))
				{
					string curGroupName = getGroupName(curGroupNode);
					if (curGroupName != null && curGroupName.ToLower().Equals(groupName.ToLower()))
					{
						groupNode = curGroupNode;
						break;
					}
				}
			}
			if (groupNode != null) {
				List<XmlNode> nodesToRemove = new List<XmlNode>();
				foreach (XmlNode permissionNode in groupNode.ChildNodes)
				{
					if (!permissionNode.LocalName.ToLower ().Equals ("permission"))
						continue;
					string innerText = getNodeVal(permissionNode);
					if (innerText != null &&
						(innerText [0] == '-' && permission [0] != '-' && innerText.Substring(1).ToLower().Equals(permission.ToLower ())) ||
						(innerText [0] != '-' && permission [0] == '-' && permission.Substring(1).ToLower().Equals(innerText.ToLower ())) ||
						(innerText.ToLower ().Equals (permission.ToLower ()))) {
						nodesToRemove.Add(permissionNode);
					}
				}
				foreach (XmlNode curNode in nodesToRemove)
				{
					groupNode.RemoveChild(curNode);
				}
			} else {
				XmlElement groupElem = this.document.CreateElement("group");
				groupElem.SetAttribute("name", groupName);
				groupsNode.AppendChild(groupElem);
				groupNode = groupElem;
			}
			XmlElement elem = this.document.CreateElement("permission");
			elem.SetAttribute ("val", permission);
			if (append)
				groupNode.AppendChild(elem);
			else
				groupNode.PrependChild(elem);
			this.reload(document);
			this.document.Save(this.xmlPath);
		}
		/// <summary>
		/// Adds a permission to a group.
		/// </summary>
		/// <param name="groupName">The name of the group.</param>
		/// <param name="permission">The permission to add.</param>
		public void addPermissionToGroup(string groupName, string permission)
		{
			addPermissionToGroup(groupName, permission, true);
		}
		/// <summary>
		/// Removes a permission from a group.
		/// </summary>
		/// <param name="groupName">The name of the group.</param>
		/// <param name="permission">The permission to remove.</param>
		public void removePermissionFromGroup(string groupName, string permission)
		{
			this.removePermissionFromDictionary(this.permissionsByGroup, groupName, permission);
			XmlNode groupNode = null;
			foreach (XmlNode curGroupNode in groupsNode.ChildNodes)
			{
				if (curGroupNode.LocalName.ToLower().Equals("group"))
				{
					string curGroupName = getGroupName(curGroupNode);
					if (curGroupName != null && curGroupName.ToLower().Equals(groupName.ToLower()))
					{
						groupNode = curGroupNode;
					}
				}
			}
			if (groupNode != null)
			{
				List<XmlNode> nodesToRemove = new List<XmlNode>();
				foreach (XmlNode curSubNode in groupNode.ChildNodes)
				{
					string innerText = getNodeVal(curSubNode);
					if (curSubNode.LocalName.ToLower().Equals("permission") && (innerText == null || innerText.ToLower().Equals (permission.ToLower())))
					{
						nodesToRemove.Add(curSubNode);
					}
				}
				foreach (XmlNode curNode in nodesToRemove)
				{
					groupNode.RemoveChild(curNode);
				}
				this.reload(document);
				this.document.Save(this.xmlPath);
			}
			else
				this.reload(document);
		}

		/// <summary>
		/// Adds a permission to a sender.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="permission">The permission to add.</param>
		/// <param name="append">If <c>true</c>, append the permission, otherwise prepend the permission.</param>
		public void addPermissionToSender(string senderId, string permission, bool append)
		{
			if (!this.permissionsBySender.ContainsKey (senderId)) {
				this.permissionsBySender.Add(senderId, new List<String>());
			}
			this.addPermissionToDictionary(this.permissionsBySender, senderId, permission, append ? -1 : 0);

			XmlNode senderNode = null;
			foreach (XmlNode curGroupNode in membersNode.ChildNodes)
			{
				if (curGroupNode.LocalName.ToLower().Equals("member"))
				{
					string memberId = getMemberId(curGroupNode);
					if (memberId != null && memberId.ToLower().Equals(senderId.ToLower())) {
						senderNode = curGroupNode;
						break;
					}
				}
			}
			if (senderNode != null) {
				List<XmlNode> nodesToRemove = new List<XmlNode>();
				foreach (XmlNode permissionNode in senderNode.ChildNodes) {
					if (!permissionNode.LocalName.ToLower().Equals ("permission"))
						continue;
					string innerText = getNodeVal(permissionNode);
					if (innerText != null &&
						(innerText [0] == '-' && permission [0] != '-' && innerText.Substring (1).ToLower ().Equals (permission.ToLower ())) ||
						(innerText [0] != '-' && permission [0] == '-' && permission.Substring (1).ToLower ().Equals (innerText.ToLower ())) ||
						(innerText.ToLower ().Equals (permission.ToLower ()))) {
						nodesToRemove.Add(permissionNode);
					}
				}
				foreach (XmlNode curNode in nodesToRemove)
				{
					senderNode.RemoveChild(curNode);
				}
			} else {
				XmlElement memberElem = this.document.CreateElement("member");
				memberElem.SetAttribute("id", senderId);
				membersNode.AppendChild(memberElem);
				senderNode = memberElem;
			}
			XmlElement elem = this.document.CreateElement("permission");
			elem.SetAttribute ("val", permission);
			if (append)
				senderNode.AppendChild(elem);
			else
				senderNode.PrependChild(elem);
			this.document.Save(this.xmlPath);
		}
		/// <summary>
		/// Adds a permission to a sender.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="permission">The permission to add.</param>
		public void addPermissionToSender(string senderId, string permission)
		{
			this.addPermissionToSender(senderId, permission, true);
		}

		/// <summary>
		/// Removes a permission from a sender.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="permission">The permission to remove.</param>
		public void removePermissionFromSender(string senderId, string permission)
		{
			this.removePermissionFromDictionary(this.permissionsBySender, senderId, permission);
			XmlNode senderNode = null;
			foreach (XmlNode curGroupNode in membersNode.ChildNodes)
			{
				if (curGroupNode.LocalName.ToLower().Equals("member"))
				{
					string memberId = getMemberId(curGroupNode);
					if (memberId != null && memberId.ToLower().Equals(senderId.ToLower())) {
						senderNode = curGroupNode;
						break;
					}
				}
			}
			if (senderNode != null)
			{
				List<XmlNode> nodesToRemove = new List<XmlNode>(senderNode.ChildNodes.Count);
				foreach (XmlNode curSubNode in senderNode.ChildNodes)
				{
					string innerText = getNodeVal(curSubNode);
					if (curSubNode.LocalName.ToLower().Equals("permission") && (innerText == null || innerText.ToLower().Equals(permission.ToLower())))
					{
						nodesToRemove.Add(curSubNode);
					}
				}
				foreach (XmlNode curNodeToRemove in nodesToRemove)
				{
					senderNode.RemoveChild (curNodeToRemove);
				}
				nodesToRemove.Clear();
				this.document.Save(this.xmlPath);
			}
			this.reload(document);
		}

		/// <summary>
		/// Adds a sender to a permission group.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="groupName">The group's name.</param>
		/// <param name="append">If <c>true</c>, append the group import, otherwise prepend it.</param>
		public void addSenderToGroup(string senderId, string groupName, bool append)
		{
			XmlNode groupNode = null;
			foreach (XmlNode curGroupNode in groupsNode.ChildNodes)
			{
				if (curGroupNode.LocalName.ToLower().Equals("group"))
				{
					string curGroupName = this.getMemberId(curGroupNode);
					if (curGroupName != null && curGroupName.Equals(senderId)) {
						groupNode = curGroupNode;
						break;
					}
				}
			}
			XmlNode senderNode = null;
			foreach (XmlNode curMemberNode in membersNode.ChildNodes)
			{
				if (curMemberNode.LocalName.ToLower().Equals("member"))
				{
					string curMemberId = this.getMemberId(curMemberNode);
					if (curMemberId != null && curMemberId.Equals(senderId)) {
						senderNode = curMemberNode;
						break;
					}
				}
			}

			if (groupNode != null && senderNode != null) {
				List<XmlNode> nodesToRemove = new List<XmlNode>();
				foreach (XmlNode curSubNode in senderNode.ChildNodes)
				{
					string innerText = getNodeVal(curSubNode);
					if (curSubNode.LocalName.ToLower ().Equals ("fromgroup") && (innerText != null || innerText.ToLower().Equals(groupName.ToLower())))
					{
						nodesToRemove.Add(curSubNode);
					}
				}
				foreach (XmlNode curNode in nodesToRemove)
				{
					senderNode.RemoveChild(curNode);
				}
			} else if (groupNode != null) {
				XmlElement memberElem = this.document.CreateElement("member");
				memberElem.SetAttribute ("id", senderId);
				membersNode.AppendChild(memberElem);
				senderNode = memberElem;
			} else {
				throw new Exception("The permissions group '" + groupName + "' was not found!");
			}
			XmlElement groupElem = this.document.CreateElement("fromgroup");
			groupElem.SetAttribute("val", groupName);
			senderNode.AppendChild(groupElem);
			this.document.Save(this.xmlPath);

			List<string> groupPermissions = null;
			foreach (string curGroupName in this.permissionsByGroup.Keys)
			{
				if (curGroupName.ToLower().Equals(groupName))
				{
					if (!this.permissionsByGroup.TryGetValue(curGroupName, out groupPermissions))
						groupPermissions = null;
					break;
				}
			}
			if (groupPermissions != null)
			{
				foreach (string curPermission in groupPermissions)
				{
					this.addPermissionToDictionary(this.permissionsBySender, senderId, curPermission, append ? -1 : 0);
				}
			}
		}
		/// <summary>
		/// Adds a sender to a permission group.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="groupName">The group's name.</param>
		public void addSenderToGroup(string senderId, string groupName)
		{
			addSenderToGroup(senderId, groupName, true);
		}

		/// <summary>
		/// Removes a sender from a permission group.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="groupName">The group's name.</param>
		public bool removeSenderFromGroup(string senderId, string groupName)
		{
			XmlNode groupNode = null;
			foreach (XmlNode curGroupNode in groupsNode.ChildNodes)
			{
				if (curGroupNode.LocalName.ToLower().Equals("group"))
				{
					string curGroupName = this.getMemberId(curGroupNode);
					if (curGroupName != null && curGroupName.Equals(senderId)) {
						groupNode = curGroupNode;
						break;
					}
				}
			}
			XmlNode senderNode = null;
			foreach (XmlNode curMemberNode in membersNode.ChildNodes)
			{
				if (curMemberNode.LocalName.ToLower().Equals("member"))
				{
					string curMemberId = this.getMemberId(curMemberNode);
					if (curMemberId != null && curMemberId.Equals(senderId)) {
						senderNode = curMemberNode;
						break;
					}
				}
			}

			if (groupNode != null && senderNode != null) {
				List<XmlNode> nodesToRemove = new List<XmlNode>();
				foreach (XmlNode curSubNode in senderNode.ChildNodes)
				{
					string innerText = getNodeVal(curSubNode);
					if (curSubNode.LocalName.ToLower ().Equals ("fromgroup") && (innerText != null || innerText.ToLower().Equals(groupName.ToLower()))) 
					{
						nodesToRemove.Add(curSubNode);
					}
				}
				foreach (XmlNode curNode in nodesToRemove)
				{
					senderNode.RemoveChild(curNode);
				}
				this.reload(document);
				return true;
			} else {
				return false;//throw new Exception("The permissions group '" + groupName + "' was not found!");
			}
		}

		private class permissionInfo
		{
			public int index;
			public string permission;
			public permissionInfo(int index, string permission)
			{
				this.index = index;
				this.permission = permission;
			}
		};

		private int getHighestPermissionIndex(Dictionary<string,List<string>> permissionDict, string id, string permission)
		{
			string idLower = id.ToLower();
			List<string> senderPermissions = null; string permLower = permission.ToLower();
			foreach (KeyValuePair<string,List<string>> curSenderPerms in permissionDict) {
				if (curSenderPerms.Key.ToLower().Equals(idLower)) {
					senderPermissions = curSenderPerms.Value;
				}
			}
			if (senderPermissions == null)
				return -1;
			for (int i = senderPermissions.Count-1; i >= 0; i--)
			{
				string curPermission = senderPermissions[i];
				string curPermLower = curPermission.ToLower();
				if (curPermLower.Equals(permLower))
				{
					return i;
				}
			}
			return -1;
		}
		private bool objectIsAllowedTo(Dictionary<string,List<string>> permissionDict, string id, string permission, out bool explicitly)
		{
			string[] permissionSub = permission.Split(new string[]{ "." }, StringSplitOptions.None);
			string[] permissions = new string[permissionSub.Length + 1];

			for (int i = 0; i < permissionSub.Length; i++) {
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				if (i == 0 && permission.StartsWith ("-"))
					builder.Append("-");
				for (int i_ = 0; i_ < i; i_++) {
					builder.Append(permissionSub[i_]);
					builder.Append(".");
				}
				builder.Append("*");
				permissions[i] = builder.ToString();
			}
			permissions[permissions.Length - 1] = permission;

			List<permissionInfo> permissionList = new List<permissionInfo>(permissions.Length);
			foreach (string curPermission in permissions)
			{
				string curPerm = (curPermission.StartsWith ("-") ? curPermission.Substring(1) : curPermission);
				int permissionIndOn = getHighestPermissionIndex(permissionDict, id, curPerm);
				if (permissionIndOn != -1)
					permissionList.Add(new permissionInfo(permissionIndOn, curPerm));
				curPerm = (curPermission.StartsWith ("-") ? curPermission : ("-"+curPermission));
				int permissionIndOff = getHighestPermissionIndex(permissionDict, id, curPerm);
				if (permissionIndOff != -1)
					permissionList.Add(new permissionInfo(permissionIndOff, curPerm));
			}
			permissionInfo[] permissionArray = permissionList.ToArray();
			Array.Sort(permissionArray, new permissionInfoComparer());

			if (permissionArray.Length == 0)
			{
				explicitly = false;
				return false;
			}

			explicitly = true;
			string explicitPermission = permissionArray[permissionArray.Length - 1].permission;
			return (explicitPermission.StartsWith("-") && permission.StartsWith("-")) || (!explicitPermission.StartsWith("-") && !permission.StartsWith("-"));
		}

		private class permissionInfoComparer : System.Collections.Generic.IComparer<permissionInfo> {
			public int Compare(permissionInfo x, permissionInfo y)
			{
				return ((permissionInfo)x).index - ((permissionInfo)y).index;
			}
		}
		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the sender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="permission">The permission.</param>
		/// <param name="explicitly">(Out) Set to <c>true</c>, if the permission was set explicitly (- before a permission means explicitly disallowed), <c>false</c> otherwise.</param> 
		public bool senderIsAllowedTo(string senderId, string permission, out bool explicitly)
		{
			return objectIsAllowedTo(this.permissionsBySender, senderId, permission, out explicitly);
			/*//explicitly = false;

			if (permission.Length <= 0 || permission[0] == '-')
				throw new ArgumentException("The permission name is invalid!");
			List<string> senderPermissions = null; string permLower = permission.ToLower();
			foreach (KeyValuePair<string,List<string>> curSenderPerms in this.permissionsBySender) {
				if (curSenderPerms.Key.Equals(senderId)) {
					senderPermissions = curSenderPerms.Value;
				}
			}
			if (senderPermissions != null)
			{
				bool isAllowed = false;
				foreach (string curPermission in senderPermissions)
				{
					string curPermLower = curPermission.ToLower();
					if (curPermLower.Equals(permLower))
					{
						explicitly = true;
						isAllowed = true;
					}
					else if (curPermLower.StartsWith("-") && curPermLower.Substring(1).Equals(permLower))
					{
						explicitly = true;
						isAllowed = false;
					}
				}
				return isAllowed;
			}
			return false;*/
		}

		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the sender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="permission">The permission.</param>
		public bool senderIsAllowedTo(string senderId, string permission)
		{
			bool tmp;
			return senderIsAllowedTo(senderId, permission, out tmp);
		}

		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the sender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="groupName">The group's name.</param>
		/// <param name="permission">The permission.</param>
		/// <param name="explicitly">(Out) Set to <c>true</c>, if the permission was set explicitly (- before a permission means explicitly disallowed), <c>false</c> otherwise.</param> 
		public bool groupIsAllowedTo(string groupName, string permission, out bool explicitly)
		{
			return objectIsAllowedTo(this.permissionsByGroup, groupName, permission, out explicitly);
		}

		/// <summary>
		/// Gets whether a sender has the specified permission.
		/// </summary>
		/// <returns><c>true</c>, if the sender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="groupName">The group's name.</param>
		/// <param name="permission">The permission.</param>
		public bool groupIsAllowedTo(string groupName, string permission)
		{
			bool tmp;
			return groupIsAllowedTo(groupName, permission, out tmp);
		}

		private List<string> readPermissions(XmlNode permissionsNode)
		{
			if (permissionsNode == null)
				return null;
			List<string> ret = new List<String>();
			if (permissionsNode.HasChildNodes)
			{
				foreach (XmlNode permNode in permissionsNode.ChildNodes)
				{
					if (permNode.LocalName.ToLower().Equals("permission"))
					{
						string innerText = getNodeVal(permNode);
						if (innerText != null)
							ret.Add(innerText);
					}
					if (permNode.LocalName.ToLower().Equals("fromgroup"))
					{
						string innerText = getNodeVal(permNode);
						List<string> groupPerms = this.readPermissions(this.getGroupByName(innerText));
						/*						if (innerText != null) {
							foreach (string groupName in this.permissionsByGroup.Keys) {
								if (groupName.ToLower().Equals(innerText.ToLower())) {
									this.permissionsByGroup.TryGetValue(groupName, out groupPerms);
									break;
								}
							}
						}*/

						if (groupPerms != null)
							ret.AddRange(groupPerms);
						else
							Log.Warning("[SK] A <fromgroup> node in the permissions.xml doesn't contain a valid group name!");
					}
				}
			}
			for (int i = 0; i < ret.Count; i++)
			{
				if ((ret[i].Length <= 0) || (ret[i].Length == 1 && ret[i][0] == '-')) {
					//Log.Out("removing " + ret[i] + " (invalid)");
					ret.RemoveAt(i); i--;
					continue;
				}
				for (int i_ = i+1; i_ < ret.Count; i_++ )
				{
					if (ret[i_].Length <= 0) {
						//Log.Out("removing " + ret[i_] + " (invalid)");
						ret.RemoveAt(i_); i_--;
						continue;
					}
					//Log.Out("perm " + ret[i] + "; " + ret[i_]);
					if (ret[i_].ToLower().Equals(ret[i].ToLower()))
					{
						//Log.Out("removing " + ret[i_] + " (duplicate)");
						ret.RemoveAt(i_);
						i_--;
						continue;
					}
					if (ret[i_].Length == 1) {
						if (ret[i_].StartsWith ("-"))
						{
							//Log.Out("removing " + ret[i_] + " (invalid)");
							ret.RemoveAt(i_);
							i_--;
						}
						continue;
					}
					if ((ret[i_].StartsWith("-") && !ret[i].StartsWith("-") && ret[i_].Substring(1).ToLower().Equals(ret[i].ToLower())) ||
						(!ret[i_].StartsWith("-") && ret[i].StartsWith("-") && ret[i].Substring(1).ToLower().Equals(ret[i_].ToLower()))) {
						//Log.Out("removing " + ret[i] + " (conflict with " + ret[i_] + ")");
						ret.RemoveAt(i);
						i--; i_--;
						continue;
					}
				}
			}
			/*			for (int i = 0; i < ret.Count; i++) {
				Log.Out("perm " + ret[i]);
			}*/
			return ret;
		}

		private string getNodeVal(XmlNode _node)
		{
			foreach (XmlNode memberInfo in _node.Attributes)
			{
				if (memberInfo.LocalName.ToLower().Equals("val"))
				{
					return memberInfo.Value;
				}
			}
			return null;
		}
		private string getGroupName(XmlNode _group)
		{
			foreach (XmlNode memberInfo in _group.Attributes)
			{
				if (memberInfo.LocalName.ToLower().Equals("name"))
				{
					return memberInfo.Value;
				}
			}
			return null;
		}
		private string getMemberId(XmlNode _member)
		{
			foreach (XmlNode memberInfo in _member.Attributes)
			{
				if (memberInfo.LocalName.ToLower().Equals("id"))
				{
					return memberInfo.Value;
				}
			}
			return null;
		}

		private XmlNode getGroupByName(string name)
		{
			foreach (XmlNode curGroup in this.groupsNode.ChildNodes) {
				if (curGroup.LocalName.ToLower ().Equals ("group")) {
					string curGroupName = this.getGroupName(curGroup);
					if (curGroupName != null && curGroupName.ToLower().Equals(name.ToLower()))
						return curGroup;
				}
			}
			return null;
		}

		private void readGroups()
		{
			foreach (XmlNode groupNode in groupsNode.ChildNodes)
			{
				if (groupNode.LocalName.ToLower().Equals("group"))
				{
					string groupName = this.getGroupName(groupNode);
					if (groupName != null)
					{
						//Log.Out("permgroup " + groupName + " :");
						List<string> permissions = readPermissions(groupNode);
						permissionsByGroup.Add(groupName, permissions);
					} 
					else
					{
						Log.Warning("[SK]  A <group> node in the permissions.xml doesn't contain a valid name attribute!");
					}
				}
			}
		}

		private void readMembers()
		{
			foreach (XmlNode groupNode in membersNode.ChildNodes)
			{
				if (groupNode.LocalName.ToLower().Equals("member"))
				{
					string id = this.getMemberId(groupNode);
					if (id != null)
					{
						//Log.Out("permmember " + id + " :");
						List<string> permissions = readPermissions(groupNode);
						permissionsBySender.Add(id, permissions);
					}
					else
					{
						Log.Warning("[SK]  A <member> node in the permissions.xml doesn't contain a valid member identifier!");
					}
				}
			}
		}

		private void addPermissionToDictionary(Dictionary<string,List<string>> dictionary, string itemName, string permission, int at)
		{
			if (itemName == null || permission == null || dictionary == null)
				return;
			if (permission.Length <= 0 || (permission.Length == 1 && permission [0] == '-'))
				return;
			List<string> itemPermissions = null;
			foreach (string curGroupName in dictionary.Keys)
			{
				if (itemName.ToLower().Equals(curGroupName.ToLower())) {
					dictionary.TryGetValue(curGroupName, out itemPermissions);
					break;
				}
			}
			if (itemPermissions == null)
				return;
			if (at == -1)
				at = itemPermissions.Count;
			bool foundPermission = false;
			for (int i = 0; i < itemPermissions.Count; i++)
			{
				string curPermission = itemPermissions[i];
				if (curPermission.ToLower().Equals(permission.ToLower()))
				{
					foundPermission = true;
					break;
				}
				if (curPermission.StartsWith("-") && curPermission.Length > 1 && curPermission.Substring(1).ToLower().Equals(permission.ToLower())) {
					foundPermission = true;
					itemPermissions.RemoveAt(i);
					itemPermissions.Insert(at, permission);
					break;
				}
				if (permission.StartsWith("-") && permission.Length > 1 && permission.Substring(1).ToLower().Equals(curPermission.ToLower())) {
					foundPermission = true;
					itemPermissions.RemoveAt(i);
					itemPermissions.Insert(at, permission);
					break;
				}
			}
			if (!foundPermission)
				itemPermissions.Insert(at, permission);
		}
		private void removePermissionFromDictionary(Dictionary<string,List<string>> dictionary, string itemName, string permission)
		{
			if (itemName == null || permission == null || dictionary == null)
				return;
			List<string> groupPermissions = null;
			foreach (string curGroupName in this.permissionsByGroup.Keys)
			{
				if (itemName.ToLower().Equals(curGroupName.ToLower())) {
					dictionary.TryGetValue(curGroupName, out groupPermissions);
					break;
				}
			}
			if (groupPermissions == null)
				return;
			for (int i = 0; i < groupPermissions.Count; i++)
			{
				if (groupPermissions[i].ToLower().Equals(permission.ToLower()))
				{
					groupPermissions.RemoveAt(i);
				}
			}
		}
	}
}

