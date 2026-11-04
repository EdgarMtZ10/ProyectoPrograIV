// Ejecutar: mongosh --file autenticacion.js
// Crea la base NovaOnline, la colección usuarios, indices unicos y 2 usuarios iniciales.
// usuario y contrasena se guardan CIFRADOS (AES, misma llave del simulador).
// Se puede correr varias veces sin duplicar datos.

const bd = db.getSiblingDB("NovaOnline");

if (!bd.getCollectionNames().includes("usuarios")) {
  bd.createCollection("usuarios");
}

bd.usuarios.createIndex({ identificacion: 1 }, { unique: true, name: "ux_identificacion" });
bd.usuarios.createIndex({ usuario: 1 }, { unique: true, name: "ux_usuario" });

// estado: 1 activo, 2 inactivo | tipo: 1 empleado, 2 cliente
// CAMBIAR la identificación del cliente por una real del verificador (MySQL).
const iniciales = [
  {
    identificacion: "101110111",
    nombre: "Admin",
    primerApellido: "Nova",
    segundoApellido: "Online",
    correo: "admin@novaonline.cr",
    usuario: "oBE87gjbotORkFKy+qEjbQ==",        // admin
    contrasena: "Xp97QO3iCtSEs+KUOUem/g==",     // Admin#Nova2026
    estado: 1,
    tipo: 1
  },
  {
    identificacion: "202220222",
    nombre: "Cliente",
    primerApellido: "Prueba",
    segundoApellido: "Nova",
    correo: "cliente1@novaonline.cr",
    usuario: "5POyMTcR+cqTjXARXLSWMQ==",        // cliente1
    contrasena: "WJC8OUqOuP5nOuzTVb+BBg==",     // Cliente#Nova26
    estado: 1,
    tipo: 2
  }
];

iniciales.forEach(function (u) {
  bd.usuarios.updateOne({ identificacion: u.identificacion }, { $setOnInsert: u }, { upsert: true });
});

print("Usuarios en la colección: " + bd.usuarios.countDocuments({}));