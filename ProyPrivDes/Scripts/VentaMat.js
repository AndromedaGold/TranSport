function listar() {
    $.get("VentaMat/listarVentaMat", function (data) {
        crearListado(["ID", "Nombre", "Cantidad", "Cliente", "Destino", "Precio", "Fecha de Venta"], data);
    })
}
listar();

$("#datepickerFechaVent").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

$(document).ready(function () {

    //Boton ver reporte
    $("#btnMostrarPDF").on("click", function () {
        $("#RptVentaMat").attr("src", "VentaMat/RptVentaMat").load();
    });

    //Boton descargar reporte
    $("#btnDescargarPDF").on("click", function () {
        window.open("VentaMat/RptVentaMat")
    });

});

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-ventamat' class='table'>";
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < arrayColumnas.length; i++) {
        contenido += "<td>";
        contenido += arrayColumnas[i];
        contenido += "</td>";
    }
    contenido += "<td>Acciones</td>"
    contenido += "</tr>";
    contenido += "</thead>";

    var llaves = Object.keys(data[0]);
    contenido += "<tbody>";
    for (var j = 0; j < data.length; j++) {
        contenido += "<tr>";
        for (var k = 0; k < llaves.length; k++) {
            var valorLlaves = llaves[k];
            contenido += "<td>";
            contenido += data[j][valorLlaves];
            contenido += "</td>";
        }
        var llaveID = llaves[0];
        contenido += "<td>";
        contenido += "<button class='btn btn-primary' onclick='abrirModal(" + data[j][llaveID] + ")' data-toggle='modal' data-target='#myModal'><i class='glyphicon glyphicon-edit' ></i></button> ";
        contenido += " <button class='btn btn-danger' onclick='eliminar(" + data[j][llaveID] + ")'><i class='glyphicon glyphicon-trash' ></i></button>"
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("tabla").innerHTML = contenido;
    $("#tabla-ventamat").dataTable({
        searching: false
    });
}

function borrarDatos() {
    var controles = document.getElementsByClassName("borrar");
    var ncontroles = controles.length;
    for (var i = 0; i < ncontroles; i++) {
        controles[i].value = "";
    }
}

function abrirModal(id) {
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }
    if (id == 0) {
        borrarDatos();
    } else {
        $.get("VentaMat/mostrarInfo/?id=" + id, function (data) {
            document.getElementById("txtId").value = data[0].Id;
            document.getElementById("txtNombreMat").value = data[0].NombreMat;
            document.getElementById("txtCantidad").value = data[0].Cantidad;
            document.getElementById("txtCliente").value = data[0].Cliente;
            document.getElementById("txtDestino").value = data[0].Destino;
            document.getElementById("txtPrecio").value = data[0].Precio;
            document.getElementById("datepickerFechaVent").value = data[0].FechaVent;
            document.getElementById("txtHabilitado").value = data[0].Habilitado;
            document.getElementById("txtRentaMaq").value = data[0].IdRentaMaq;
            document.getElementById("txtOperador").value = data[0].IdOperador;

        });
    }
}

function datosObligatorios() {
    var exito = true;
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        if (controlesObligatorios[i].value == "") {
            exito = false;
            controlesObligatorios[i].parentNode.classList.add("error");
        } else {
            controlesObligatorios[i].parentNode.classList.remove("error");
        }
    }
    return exito;
}

function eliminar(id) {
    if (confirm("¿Desea eliminar el dato?") == 1) {
        $.get("VentaMat/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrio un error");
            } else {
                alert("El dato se eliminó correctamente");
                listar();
            }
        })
    }
}

var btnBuscar = document.getElementById("btnBuscar");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("VentaMat/buscarVentaMat/?nombre=" + nombre, function (data) {
        crearListado(["ID", "Nombre", "Cantidad", "Cliente", "Destino", "Precio", "Fecha de Venta"], data);
    })
}

var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    $.get("VentaMat/listarVentaMat", function (data) {
        crearListado(["ID", "Nombre", "Cantidad", "Cliente", "Destino", "Precio", "Fecha de Venta"], data);
    });
    document.getElementById("txtnombre").value = "";
}

function Agregar() {
    if (datosObligatorios() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtId").value;
        var nombre = document.getElementById("txtNombreMat").value;
        var cantidad = document.getElementById("txtCantidad").value;
        var cliente = document.getElementById("txtCliente").value;
        var destino = document.getElementById("txtDestino").value;
        var precio = document.getElementById("txtPrecio").value;
        var fecha = document.getElementById("datepickerFechaVent").value;
        var habilitado = document.getElementById("txtHabilitado").value;
        var renta = document.getElementById("txtRentaMaq").value;
        var operador = document.getElementById("txtOperador").value;

        frm.append("Id", id);
        frm.append("NombreMat", nombre);
        frm.append("Cantidad", cantidad);
        frm.append("Cliente", cliente);
        frm.append("Destino", destino);
        frm.append("Precio", precio);
        frm.append("FechaVent", fecha);
        frm.append("Habilitado", habilitado);
        frm.append("IdRentaMaq", renta);
        frm.append("IdOperador", operador);

        if (confirm("Desea guardar los cambios") == 1) {
            $.ajax({
                type: "POST",
                url: "VentaMat/guardar",
                data: frm,
                contenType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("El dato se guardo correctamente");
                        listar();
                        document.getElementById("btnCancelar").click();
                    } else {
                        if (data == -1) {
                            alert("El dato ya existe");
                        }
                        alert("Ocurrio un error");
                    }
                }
            });
        }
    }
}