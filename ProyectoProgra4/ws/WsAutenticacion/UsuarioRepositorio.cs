using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WsAutenticacion
{
    public class UsuarioRepositorio
    {
        private static readonly object Candado = new object();
        private static IMongoCollection<BsonDocument> _coleccion;

        private static string Config(string llave)
        {
            string v = ConfigurationManager.AppSettings[llave];
            if (string.IsNullOrWhiteSpace(v))
                throw new ConfigurationErrorsException("Falta la llave '" + llave + "' en Web.config");
            return v;
        }

        private static IMongoCollection<BsonDocument> Coleccion()
        {
            if (_coleccion == null)
            {
                lock (Candado)
                {
                    if (_coleccion == null)
                    {
                        string host = Config("MongoHost");
                        int puerto = int.Parse(Config("MongoPort"));
                        string baseDatos = Config("MongoBase");
                        string coleccion = Config("MongoColeccion");

                        var settings = new MongoClientSettings
                        {
                            Server = new MongoServerAddress(host, puerto),
                            ServerSelectionTimeout = TimeSpan.FromSeconds(5)
                        };
                        var cliente = new MongoClient(settings);
                        _coleccion = cliente.GetDatabase(baseDatos).GetCollection<BsonDocument>(coleccion);
                    }
                }
            }
            return _coleccion;
        }

        // Usuario y contraseña llegan cifrados con el mismo valor que está guardado (IV fijo).
        public BsonDocument BuscarCredenciales(string usuarioCifrado, string contrasenaCifrada, int tipo)
        {
            var f = Builders<BsonDocument>.Filter;
            var filtro = f.Eq("usuario", usuarioCifrado)
                       & f.Eq("contrasena", contrasenaCifrada)
                       & f.Eq("tipo", tipo)
                       & f.Eq("estado", 1);
            return Coleccion().Find(filtro).FirstOrDefault();
        }

        public bool ExisteIdentificacionOUsuario(string identificacion, string usuarioCifrado)
        {
            var f = Builders<BsonDocument>.Filter;
            var filtro = f.Eq("identificacion", identificacion) | f.Eq("usuario", usuarioCifrado);
            return Coleccion().CountDocuments(filtro, new CountOptions { Limit = 1 }) > 0;
        }

        public bool UsuarioEnUsoPorOtro(string usuarioCifrado, string identificacion)
        {
            var f = Builders<BsonDocument>.Filter;
            var filtro = f.Eq("usuario", usuarioCifrado) & f.Ne("identificacion", identificacion);
            return Coleccion().CountDocuments(filtro, new CountOptions { Limit = 1 }) > 0;
        }

        public BsonDocument Obtener(string identificacion)
        {
            return Coleccion().Find(Builders<BsonDocument>.Filter.Eq("identificacion", identificacion)).FirstOrDefault();
        }

        public void Insertar(BsonDocument documento)
        {
            Coleccion().InsertOne(documento);
        }

        public void Actualizar(string identificacion, UpdateDefinition<BsonDocument> cambios)
        {
            Coleccion().UpdateOne(Builders<BsonDocument>.Filter.Eq("identificacion", identificacion), cambios);
        }
    }
}