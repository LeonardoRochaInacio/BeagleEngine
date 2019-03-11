using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beagle.Pattern
{
    public sealed class Singleton<T>
    {
        static Singleton<T> _instancia;
        public static Singleton<T> Instancia
        {
            get { return _instancia ?? (_instancia = new Singleton<T>()); }
        }
        private Singleton() { }
        public string Mensagem { get; set; }
    }
}
