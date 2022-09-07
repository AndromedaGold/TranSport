$("#datepickerExtraccion").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

listar();

function listar() {
    $.get("Extraccion/listarExtraccion", function (data) {
        crearListado(["ID", "Nombre Material", "Cantidad Tons", "Fecha Extraccion", "Pureza"], data);
    })
}


var btnBuscar = document.getElementById("btnBuscar");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("Extraccion/buscarExtraccion/?nombre=" + nombre, function (data) {
        crearListado(["ID", "Nombre Material", "Cantidad Tons", "Fecha Extraccion", "Pureza"], data);
    })
}

var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    $.get("Extraccion/listarExtraccion", function (data) {
        crearListado(["ID", "Nombre Material", "Cantidad Tons", "Fecha Extraccion", "Pureza"], data);
    });
    document.getElementById("txtnombre").value = "";
}

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-extraccion' class='table'>";
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
    $("#tabla-extraccion").dataTable({
        searching: false
    });
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
        $.get("Extraccion/mostrarInfo/?id=" + id, function (data) {
            document.getElementById("txtId").value = data[0].Id;
            document.getElementById("txtNombreMaterial").value = data[0].NombreMaterial;
            document.getElementById("txtCantidadTons").value = data[0].CantidadTons;           
            document.getElementById("datepickerExtraccion").value = data[0].FechaExtraccion;
            document.getElementById("txtPureza").value = data[0].Pureza;
            document.getElementById("txtHabilitado").value = data[0].Habilitado;
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

function borrarDatos() {
    var controles = document.getElementsByClassName("borrar");
    var ncontroles = controles.length;
    for (var i = 0; i < ncontroles; i++) {  
        controles[i].value = "";
    }
}

function Agregar() {
    if (datosObligatorios() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtId").value;
        var nombre = document.getElementById("txtNombreMaterial").value;
        var cantidad = document.getElementById("txtCantidadTons").value;
        var fecha = document.getElementById("datepickerExtraccion").value;
        var pureza = document.getElementById("txtPureza").value;
        var habilitado = document.getElementById("txtHabilitado").value;

        frm.append("Id", id);
        frm.append("NombreMaterial", nombre);
        frm.append("CantidadTons", cantidad);
        frm.append("FechaExtraccion", fecha);
        frm.append("Pureza", pureza);
        frm.append("Habilitado", habilitado);

        if (confirm("¿Desea guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "Extraccion/guardar",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("El dato se guardó correctamente");
                        listar();
                        document.getElementById("btnCancelar").click();
                    } else {
                        if (data == -1) {
                            alert("La Extraccion ya existe");
                        }
                        alert("Ocurrio un error");
                    }
                }
            });
        }
    }
}

function eliminar(id) {
    if (confirm("¿Desea eliminar el dato?") == 1) {
        $.get("Extraccion/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrió un error");
            } else {
                alert("Se elimino correctamente");
                listar();
            }
        });
    }
}

$(document).ready(function () {

    //Boton ver reporte
    $("#btnMostrarPDF").on("click", function () {
        $("#RptExtraccion").attr("src", "Extraccion/RptExtraccion").load();
    });

    //Boton descargar reporte
    $("#btnDescargarPDF").on("click", function () {
        window.open("Extraccion/RptExtraccion")
    });

});
