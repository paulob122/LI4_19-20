﻿using System;
using System.Text;

namespace TesteApiConnect
{
    class Cliente : InterfaceUtilizador
    {
        public string email;
        public int nif;
        public string nome;
        public int genero;
        public string data_nascimento;
        public string localidade;
        public string categoria;

        public Cliente(string email, int nif, string nome, int genero, 
                       string data_nascimento, string localidade, string categoria)
        {
            this.email = email;
            this.nif = nif;
            this.nome = nome;
            this.genero = genero;
            this.data_nascimento = data_nascimento;
            this.localidade = localidade;
            this.categoria = categoria;
        }

        public override string ToString()
        {
            StringBuilder r = new StringBuilder();

            r.Append("\nEmail: " + this.email + ";\n");
            r.Append("Nif: " + this.nif + ";\n");
            r.Append("Nome: " + this.nome + ";\n");
            r.Append("Genero: " + this.genero + ";\n");
            r.Append("Data nascimento: " + this.data_nascimento + ";\n");
            r.Append("Localidade: " + this.localidade + ";\n");
            r.Append("Categoria: " + this.categoria + ".\n");

            return r.ToString();
        }

        public string GetEmail()
        {
            return this.email;
        }
    }
}
