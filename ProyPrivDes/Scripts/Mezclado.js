listar();
function listar() {
    $.get("Mezclado/listarMezclado", function (data) {
        crearListado(["Id", "Nombre", "Cantidad", "Finura", "Fecha Mezclado"], data);
    })
}

var nombreMat = document.getElementById("txtnombre");
nombreMat.onkeyup = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("Mezclado/buscarMezclado/?nombreMat=" + nombre, function (data) {
        crearListado(["Id", "Nombre", "Cantidad", "Finura", "Fecha Mezclado"], data);
    })
}

$(document).ready(function () {

    //Boton ver reporte
    $("#btnMostrarPDF").on("click", function () {
        $("#RptMezclado").attr("src", "Mezclado/RptMezclado").load();
    });

    //Boton descargar reporte
    $("#btnDescargarPDF").on("click", function () {
        window.open("Mezclado/RptMezclado")
    });

});

$("#datepickerMezclado").datepicker(
    {
        datedateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
)

$.get("Mezclado/listarFinura", function (data) {   
    llenarCombo(data, document.getElementById("cboxFinura"), true);
})

function llenarCombo(data, control, primerElemento) {
    var contenido = "";
    if (primerElemento == true) {
        contenido += "<option value=''>--Seleccione--</option>";
    }
    for (var i = 0; i < data.length; i++) {
        contenido += "<option value='" + data[i].ID + "'>";
        contenido += data[i].Descripcion;
        contenido += "</option>";
    }
    control.innerHTML = contenido;
}

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-mezclado' class='table'>";
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
    $("#tabla-mezclado").dataTable({
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

function abrirModal(id) {
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }
    if (id == 0) {
        borrarDatos();
    } else {
        $.get("Mezclado/mostrarInfo/?id=" + id, function (data) {
            document.getElementById("txtId").value = data[0].Id;
            document.getElementById("txtMateriaPrim").value = data[0].MateriaPrim;           
            document.getElementById("txtCantidad").value = data[0].Cantidad;
            document.getElementById("cboxFinura").value = data[0].FinuraId;
            document.getElementById("datepickerMezclado").value = data[0].Fecha;
            document.getElementById("txtHabilitado").value = data[0].Habilitado;
        });
    }
}

function eliminar(id) {
    if (confirm("¿Desea eliminar el dato?") == 1) {
        $.get("Mezclado/eliminar/?id=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrio un error");
            } else {
                alert("el dato se elimino correctamente");
                listar();
            }
        });
    }
}

function Agregar() {
    if (datosObligatorios() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtId").value;
        var materia = document.getElementById("txtMateriaPrim").value;
        var cantidad = document.getElementById("txtCantidad").value;
        var finura = document.getElementById("cboxFinura").value;
        var fecha = document.getElementById("datepickerMezclado").value;
        var habilitado = document.getElementById("txtHabilitado").value;

        frm.append("Id", id);
        frm.append("MateriaPrim", materia);
        frm.append("Cantidad", cantidad);
        frm.append("FinuraId", finura);
        frm.append("Fecha", fecha);
        frm.append("Habilitado", habilitado);

        if (confirm("¿Desea guardar los cambios?") == 1) {
            $.ajax({
                type: "POST",
                url: "Mezclado/guardar",
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
                            alert("Ese dato ya existe");
                        }
                        alert("Ocurrio un error");
                    }
                }
            });
        }
    } else { }
}