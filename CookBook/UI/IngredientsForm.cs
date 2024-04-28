﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccessLayer.Contracts;
using DataAccessLayer.Repositories;
using DomainModel.Models;

namespace CookBook.UI
{
    public partial class IngredientsForm : Form
    {
        readonly IIngredientsRepository _ingredientsRepository;
        public IngredientsForm(IIngredientsRepository ingredientsRepository)
        {
            InitializeComponent();
            _ingredientsRepository = ingredientsRepository;
        }

        private int _ingredientToEditId;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private async void AddToFridgeBtn_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                return;
            }

            Ingredient ingredient = new Ingredient(NameTxt.Text, TypeTxt.Text, WeightNum.Value, KcalPer100gNum.Value, PricePer100gNum.Value);

            AddToFridgeBtn.Enabled = false;
            await _ingredientsRepository.AddIngredient(ingredient);
            AddToFridgeBtn.Enabled = true;

            ClearAllFields();
            RefreshGridData();
        }


        private void ClearAllFields()
        {
            NameTxt.Text = default;
            TypeTxt.Text = default;
            WeightNum.Value = 1;
            KcalPer100gNum.Value = default;
            PricePer100gNum.Value = default;
            SearchTxt.Text = string.Empty;
        }
        private void IngredientsForm_Load(object sender, EventArgs e)
        {
            RefreshGridData();
            CustomizeGridAppearance();

            AddToFridgeBtn.Visible = true;
            EditIngredientBtn.Visible =false;
        }

        private async void RefreshGridData()
        {
            IngredientsGrid.DataSource = await _ingredientsRepository.GetIngredient(SearchTxt.Text);

        }

        private void CustomizeGridAppearance()
        {
            IngredientsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            IngredientsGrid.AutoGenerateColumns = false;

            DataGridViewColumn[] columns = new DataGridViewColumn[8];
            columns[0] = new DataGridViewTextBoxColumn() { DataPropertyName = "Id", Visible = false };
            columns[1] = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Name",
                HeaderText = "Name",
                Visible = true
            };
            columns[2] = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Visible = true
            };
            columns[3] = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Weight",
                HeaderText = "Weight",
                Visible = true
            };
            columns[4] = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "PricePer100g",
                HeaderText = "Price(100g)",
                Visible = true
            };
            columns[5] = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "KcalPer100g",
                HeaderText = "Kcal(100g)",
                Visible = true
            };
            columns[6] = new DataGridViewButtonColumn()
            {
                Text = "Delete",
                Name = "DeleteBtn",
                HeaderText = "",
                UseColumnTextForButtonValue = true
            };
            columns[7] = new DataGridViewButtonColumn()
            {
                Text = "Edit",
                Name = "EditBtn",
                HeaderText = "",
                UseColumnTextForButtonValue = true
            };

            IngredientsGrid.Columns.Clear();
            IngredientsGrid.Columns.AddRange(columns);
        }

        private async void SearchTxt_TextChanged(object sender, EventArgs e)
        {
            int lengthBeforePause = SearchTxt.Text.Length;

            await Task.Delay(500);

            int lengthAfterPause = SearchTxt.Text.Length;

            if (lengthBeforePause == lengthAfterPause)
                RefreshGridData();
        }

        private void ClearAllFieldsBtn_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void KcalPer100gNum_ValueChanged(object sender, EventArgs e)
        {

        }

        private bool IsValid()
        {
            bool isValid = true;
            string message = "";

            if (string.IsNullOrEmpty(NameTxt.Text))
            {
                isValid = false;
                message += "Please enter name. \n\n";
            }
            else
            {
                List<Ingredient> allIngredients = (List<Ingredient>)IngredientsGrid.DataSource;

                foreach (Ingredient ingredient in allIngredients)
                {
                    if (ingredient.Name.ToLower() == NameTxt.Text.ToLower())
                    {
                        MessageBox.Show("That ingredient already exists!", "Form not valid!");
                        return false;
                    }
                }
            }
            if (string.IsNullOrEmpty(TypeTxt.Text))
            {
                isValid = false;
                message += "Please enter type. \n\n";
            }
            if (WeightNum.Value <= 0)
            {
                isValid = false;
                message += "Weight must be greater than 0. \n\n";
            }
            if (PricePer100gNum.Value <= 0)
            {
                isValid = false;
                message += "Price must be greater than 0. \n\n";
            }
            if (KcalPer100gNum.Value < 0)
            {
                isValid = false;
                message += "Kcal must be greater than or equal to 0. \n\n";
            }
            if (!isValid)
            {
                MessageBox.Show(message, "Form not valid!");
            }
            return isValid;
        }

        private async void IngredientsGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && IngredientsGrid.CurrentCell is DataGridViewButtonCell)
            {
                Ingredient clickedIngredient = (Ingredient)IngredientsGrid.Rows[e.RowIndex].DataBoundItem;

                if (IngredientsGrid.CurrentCell.OwningColumn.Name == "DeleteBtn")
                {
                    await _ingredientsRepository.DeleteIngredient(clickedIngredient);
                    RefreshGridData();
                }
                else if (IngredientsGrid.CurrentCell.OwningColumn.Name == "EditBtn")
                {
                    FillFormforEdit(clickedIngredient);
                }
            }
        }

        private void FillFormforEdit(Ingredient clickedIngredient)
        {

            _ingredientToEditId = clickedIngredient.Id;

            NameTxt.Text = clickedIngredient.Name;
            TypeTxt.Text = clickedIngredient.Type;
            WeightNum.Value = clickedIngredient.Weight;
            PricePer100gNum.Value = clickedIngredient.PricePer100g;
            KcalPer100gNum.Value = clickedIngredient.KcalPer100g;

            AddToFridgeBtn.Visible = false;
            EditIngredientBtn.Visible = true;
        }

        private async void EditIngredientBtn_Click(object sender, EventArgs e)
        {
            if(!IsValid())
            { return; }

            Ingredient ingredient = new Ingredient(NameTxt.Text, TypeTxt.Text, WeightNum.Value, KcalPer100gNum.Value, PricePer100gNum.Value, _ingredientToEditId);
                ingredient.Id = _ingredientToEditId;

            await _ingredientsRepository.EditIngredient(ingredient);

             ClearAllFields();
             RefreshGridData();

            EditIngredientBtn.Visible = false;
            AddToFridgeBtn.Visible = true;

            _ingredientToEditId = 0;
        }
    }
}

