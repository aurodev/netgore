﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetGore;
using NetGore.EditorTools;
using NetGore.IO;

namespace DemoGame.MapEditor
{
    public partial class InputNewMapIDForm : Form
    {
        MapID? _value;

        /// <summary>
        /// Gets the currently entered <see cref="MapID"/> value.
        /// </summary>
        public MapID? Value { get { return _value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputNewMapIDForm"/> class.
        /// </summary>
        public InputNewMapIDForm() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputNewMapIDForm"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public InputNewMapIDForm(MapID? defaultValue)
        {
            InitializeComponent();

            if (defaultValue.HasValue)
                txtID.Text = defaultValue.Value.ToString();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the Click event of the btnFindFree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnFindFree_Click(object sender, EventArgs e)
        {
            var freeID = MapBase.GetNextFreeIndex(ContentPaths.Dev);
            txtID.Text = freeID.ToString();
        }

        /// <summary>
        /// Handles the Click event of the btnAccept control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (!_value.HasValue)
            {
                MessageBox.Show("You must enter a valid map ID first.", "Invalid map ID", MessageBoxButtons.OK);
                return;
            }

            if (MapBase.IsValidMapFile(MapBase.GetMapFilePath(ContentPaths.Dev, _value.Value)))
            {
                const string msg = "That map ID is already in use. Are you sure you wish to use it? The existing map will be overwritten and lost.";
                if (MessageBox.Show(msg, "Overwrite map?", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the TextChanged event of the txtID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtID_TextChanged(object sender, EventArgs e)
        {
            int i;
            if (!int.TryParse(txtID.Text, out i))
            {
                _value = null;
            }
            else
            {
                try
                {
                    _value = new MapID(i);
                }
                catch (ArgumentException)
                {
                    _value = null;
                }
            }

            Color newColor;
            if (!_value.HasValue)
                newColor = EditorColors.Error;
            else
                newColor = EditorColors.Normal;

            if (txtID.BackColor != newColor)
                txtID.BackColor = newColor;
        }
    }
}