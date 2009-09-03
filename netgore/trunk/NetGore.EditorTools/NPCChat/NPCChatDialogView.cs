using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NetGore.EditorTools.NPCChat
{
    public class NPCChatDialogView : TreeView
    {
        Color _nodeForeColorGoTo = Color.Blue;
        Color _nodeForeColorNormal = Color.Black;
        Color _nodeForeColorBranch = Color.DarkRed;
        Color _nodeForeColorResponse = Color.Green;
        EditorNPCChatDialog _npcChatDialog;

        [Description("The font color of GOTO nodes.")]
        public Color NodeForeColorGoTo
        {
            get { return _nodeForeColorGoTo; }
            set { _nodeForeColorGoTo = value; }
        }

        [Description("The font color of branch chat dialog nodes.")]
        public Color NodeForeColorBranch
        {
            get { return _nodeForeColorBranch; }
            set { _nodeForeColorBranch = value; }
        }

        [Description("The font color of normal chat dialog nodes.")]
        public Color NodeForeColorNormal
        {
            get { return _nodeForeColorNormal; }
            set { _nodeForeColorNormal = value; }
        }

        [Description("The font color of dialog response value nodes.")]
        public Color NodeForeColorResponse
        {
            get { return _nodeForeColorResponse; }
            set { _nodeForeColorResponse = value; }
        }

        public EditorNPCChatDialog NPCChatDialog
        {
            get { return _npcChatDialog; }
            set
            {
                if (_npcChatDialog == value)
                    return;

                _npcChatDialog = value;
                Nodes.Clear();
                UpdateItems();
            }
        }

        TreeNode CreateTreeNode(EditorNPCChatDialogItem item, TreeNode parent)
        {
            // Check for a node that already handles this item
            TreeNodeCollection nodeCollection = parent != null ? parent.Nodes : Nodes;
            foreach (TreeNode childNode in nodeCollection.OfType<TreeNode>())
            {
                if (childNode.Tag == item)
                    return childNode;
            }

            // Create the new node
            TreeNode ret = new TreeNode { Tag = item };
            item.TreeNodes.Add(ret);
            item.OnChange += HandleChangeDialogItem;

            if (parent != null)
                parent.Nodes.Add(ret);
            else
                Nodes.Add(ret);

            UpdateTreeNode(ret);

            return ret;
        }

        TreeNode CreateTreeNode(EditorNPCChatResponse item, TreeNode parent, out bool createdNew)
        {
            // Check for a node that already handles this item
            foreach (TreeNode childNode in parent.Nodes.OfType<TreeNode>())
            {
                if (childNode.Tag == item)
                {
                    createdNew = false;
                    return childNode;
                }
            }

            // Create the new node
            TreeNode ret = new TreeNode { Tag = item };
            item.TreeNodes.Add(ret);
            item.OnChange += HandleChangeResponse;
            parent.Nodes.Add(ret);
            UpdateTreeNode(ret);

            createdNew = true;
            return ret;
        }

        void CreateTreeNode(TreeNode item, TreeNode parent)
        {
            // Check for a node that already handles this item
            foreach (TreeNode childNode in parent.Nodes.OfType<TreeNode>())
            {
                if (childNode.Tag == item)
                    return;
            }

            // Create the new node
            EditorNPCChatDialogItem redirectTo = (EditorNPCChatDialogItem)item.Tag;

            TreeNode ret = new TreeNode { Tag = item };
            redirectTo.TreeNodes.Add(ret);
            redirectTo.OnChange += HandleChangeDialogItem;
            parent.Nodes.Add(ret);
            UpdateTreeNode(ret);
        }

        public TreeNode FindNode(Predicate<TreeNode> p)
        {
            return FindTreeNode(Nodes, p);
        }

        static TreeNode FindTreeNode(TreeNodeCollection nodes, Predicate<TreeNode> predicate)
        {
            foreach (TreeNode node in nodes.OfType<TreeNode>())
            {
                if (predicate(node))
                    return node;

                return FindTreeNode(node.Nodes, predicate);
            }

            return null;
        }

        IEnumerable<TreeNode> GetParents(TreeNode node)
        {
            TreeNode current = node;
            while (current.Parent != null)
            {
                yield return current.Parent;
                current = current.Parent;
            }
        }

        static string GetTextForDialogItem(EditorNPCChatDialogItem item)
        {
            string text = item.Title;
            if (string.IsNullOrEmpty(text))
            {
                text = item.Text;
                if (text.Length > 100)
                    text = text.Substring(0, 100);
            }

            return text;
        }

        void HandleChangeDialogItem(EditorNPCChatDialogItem dialogItem)
        {
            foreach (TreeNode node in dialogItem.TreeNodes)
            {
                UpdateTreeNode(node);
            }
        }

        void HandleChangeResponse(EditorNPCChatResponse response)
        {
            foreach (TreeNode node in response.TreeNodes)
            {
                UpdateTreeNode(node);
            }
        }

        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                TreeNode tagAsTreeNode = e.Node.Tag as TreeNode;
                if (tagAsTreeNode != null)
                    SelectedNode = tagAsTreeNode;
            }

            base.OnNodeMouseDoubleClick(e);
        }

        void RecursiveUpdateItems(TreeNode node, TreeNode parentNode, EditorNPCChatDialogItem item)
        {
            // Create the main node
            if (node == null)
                node = CreateTreeNode(item, parentNode);

            // Add the child response nodes
            if (item.ResponseList != null)
            {
                foreach (EditorNPCChatResponse response in item.ResponseList)
                {
                    EditorNPCChatDialogItem responseItem = NPCChatDialog.GetDialogItemCasted(response.Page);
                    ushort responsePage = response.Page;
                    TreeNode existingPageNode = FindNode(x => TreeNodeIsForDialogItem(x, responsePage));

                    bool createdNew;
                    TreeNode childNode = CreateTreeNode(response, node, out createdNew);

                    if (existingPageNode != null && createdNew)
                        CreateTreeNode(existingPageNode, childNode);

                    // This GetParents() thing will check to make sure that it is not redirecting to any of it's own parents
                    // because doing so would otherwise cause an infinite recursion of updates
                    if (responseItem != null &&
                        !GetParents(node).Any(
                             x => x.Tag is EditorNPCChatDialogItem && ((EditorNPCChatDialogItem)x.Tag).Index == responsePage))
                        RecursiveUpdateItems(existingPageNode, childNode, responseItem);
                }

                // Delete obsolete nodes
                var nodesToRemove = new List<TreeNode>(2);
                foreach (TreeNode childNode in node.Nodes.OfType<TreeNode>())
                {
                    TreeNode n = childNode;
                    if (n.Tag is EditorNPCChatResponse && !item.ResponseList.Any(x => x.TreeNodes.Contains(n)))
                        nodesToRemove.Add(n);
                }

                foreach (TreeNode removeNode in nodesToRemove)
                {
                    removeNode.Remove();
                }
            }
        }

        static bool TreeNodeIsForDialogItem(TreeNode treeNode, ushort dialogItemIndex)
        {
            EditorNPCChatDialogItem castedTag = treeNode.Tag as EditorNPCChatDialogItem;
            if (castedTag == null)
                return false;

            return castedTag.Index == dialogItemIndex;
        }

        public void UpdateItems()
        {
            if (NPCChatDialog == null)
                return;

            EditorNPCChatDialogItem initialItem = NPCChatDialog.GetInitialDialogItemCasted();
            RecursiveUpdateItems(null, null, initialItem);
        }

        public void UpdateTreeNode(TreeNode node)
        {
            TreeNode asTreeNode;
            EditorNPCChatDialogItem asItem;
            EditorNPCChatResponse asResponse;

            string text = string.Empty;
            Color foreColor = Color.Black;

            if ((asTreeNode = node.Tag as TreeNode) != null)
            {
                EditorNPCChatDialogItem redirectItem = (EditorNPCChatDialogItem)asTreeNode.Tag;
                text = string.Format("[GOTO {0}: {1}]", redirectItem.Index, GetTextForDialogItem(redirectItem));

                foreColor = NodeForeColorGoTo;
            }
            else if ((asItem = node.Tag as EditorNPCChatDialogItem) != null)
            {
                text = GetTextForDialogItem(asItem);

                if (asItem.IsBranch)
                    foreColor = NodeForeColorBranch;
                else
                    foreColor = NodeForeColorNormal;
            }
            else if ((asResponse = node.Tag as EditorNPCChatResponse) != null)
            {
                EditorNPCChatDialogItem dialogItem = (EditorNPCChatDialogItem)node.Parent.Tag;

                text = string.Format("[{0}: {1}]", dialogItem.ResponseList.IndexOf(asResponse), asResponse.Text);
                foreColor = NodeForeColorResponse;
            }

            node.Text = text;
            node.ForeColor = foreColor;
        }
    }
}