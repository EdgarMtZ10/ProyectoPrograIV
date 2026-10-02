package almacen.almacen4.datos;

import java.time.LocalDate;
import java.util.List;

/**
 * Representa el encabezado (numero de
 * factura, fecha de venta) mas la lista de productos a retirar.
 *
 * esta no trae
 * cedula juridica
 */
public class Salida {

    private final long numeroFactura;
    private final LocalDate fechaVenta;
    private final List<SalidaDetalle> detalles;

    public Salida(long numeroFactura, LocalDate fechaVenta, List<SalidaDetalle> detalles) {
        this.numeroFactura = numeroFactura;
        this.fechaVenta = fechaVenta;
        this.detalles = detalles;
    }

    public long getNumeroFactura() {
        return numeroFactura;
    }

    public LocalDate getFechaVenta() {
        return fechaVenta;
    }

    public List<SalidaDetalle> getDetalles() {
        return detalles;
    }
}