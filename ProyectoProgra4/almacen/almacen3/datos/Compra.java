package almacen.almacen3.datos;

import java.time.LocalDate;
import java.util.List;

public class Compra {
    private final long numeroIngreso;//identificador único de la compra
    private final LocalDate fechaCompra;//fecha en la que se realizó la compra
    private final String cedulaJuridica;
    private final List<CompraDetalle> detalles;//lista de detalles de la compra, cada uno representando un producto y su cantidad

    //constructor de la clase Compra que inicializa sus atributos con los valores proporcionados
    public Compra(long numeroIngreso, LocalDate fechaCompra, String cedulaJuridica, List<CompraDetalle> detalles) {
        this.numeroIngreso = numeroIngreso;
        this.fechaCompra = fechaCompra;
        this.cedulaJuridica = cedulaJuridica;
        this.detalles = detalles;
    }

    //metodos getter para acceder a los atributos de la clase Compra
    public long getNumeroIngreso() {
        return numeroIngreso;
    }

    public LocalDate getFechaCompra() {
        return fechaCompra;
    }

    public String getCedulaJuridica() {
        return cedulaJuridica;
    }

    public List<CompraDetalle> getDetalles() {
        return detalles;
    }
}