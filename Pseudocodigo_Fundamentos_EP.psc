Algoritmo TramiteDocumentario
	// Solucion en pseudocodigo para el trámite documentario
	// Arreglos paralelos: la posición i describe un mismo expediente.
	Definir codigo, fecha, tipo, plazo, estado, orden Como Entero
	Definir remitente, asunto, area Como Caracter
	Definir n, año, opcion Como Entero
	Dimension codigo[100], fecha[100], tipo[100], plazo[100], estado[100], orden[100]
	Dimension remitente[100], asunto[100], area[100]
	n <- 0
	Escribir "Año de registro (AAAA): "
	Leer año
	Repetir
		Escribir ""
		Escribir "1. Registrar  2. Buscar  3. Listar por fecha  4. Cambiar estado  0. Salir"
		Escribir "Opción: "
		Leer opcion
		Segun opcion Hacer
			1:
				RegistrarDocumento(codigo, fecha, remitente, asunto, tipo, area, plazo, estado, n, año)
			2:
				BuscarYMostrar(codigo, fecha, remitente, asunto, area, plazo, estado, n)
			3:
				ListarPorFecha(codigo, fecha, remitente, asunto, area, plazo, estado, orden, n)
			4:
				CambiarEstado(codigo, estado, n)
			0:
				Escribir "Fin del programa."
			De Otro Modo:
				Escribir "Opción no válida."
		FinSegun
	Hasta Que opcion = 0
FinAlgoritmo

// ---------- REGISTRO ----------
SubProceso RegistrarDocumento(codigo, fecha, remitente, asunto, tipo, area, plazo, estado, n Por Referencia, año)
	Definir f, t Como Entero
	Si n >= 100 Entonces
		Escribir "Capacidad máxima alcanzada."
	Sino
		Repetir
			Escribir "Fecha del documento (AAAAMMDD): "
			Leer f
		Hasta Que EsFechaValida(f)
		n <- n + 1
		Escribir "1. Solicitud de cliente  2. Contrato de servicios  3. Factura o comprobante"
		Escribir "4. Memorando, oficio o carta  5. Expediente del personal  6. Certificado o permiso"
		Repetir
			Escribir "Tipo (1 a 6): "
			Leer t
		Hasta Que t >= 1 Y t <= 6
		Escribir "Remitente: "
		Leer remitente[n]
		Escribir "Asunto: "
		Leer asunto[n]
		codigo[n] <- anio * 1000 + n
		fecha[n] <- f
		tipo[n] <- t
		area[n] <- AreaDeDerivacion(t)
		plazo[n] <- PlazoDeTipo(t)
		estado[n] <- 1
		Escribir "Registrado: ", codigo[n], " -> ", area[n]
	FinSi
FinSubProceso

Funcion ok <- EsFechaValida(f)
	Definir ok Como Logico
	Definir a, m, d Como Entero
	a <- trunc(f / 10000)
	m <- trunc((f - a * 10000) / 100)
	d <- f - a * 10000 - m * 100
	Si a >= 2000 Y a <= 2100 Y m >= 1 Y m <= 12 Y d >= 1 Y d <= 31 Entonces
		ok <- Verdadero
	Sino
		ok <- Falso
	FinSi
FinFuncion

Funcion a <- AreaDeDerivacion(t)
	Definir a Como Caracter
	Segun t Hacer
		1:
			a <- "Ventas y Marketing"
		2:
			a <- "Administración y Finanzas"
		3:
			a <- "Administración y Finanzas"
		4:
			a <- "Gerencia General"
		5:
			a <- "Recursos Humanos"
		6:
			a <- "Operaciones"
	FinSegun
FinFuncion

Funcion p <- PlazoDeTipo(t)
	Definir p Como Entero
	Segun t Hacer
		1:
			p <- 5
		2:
			p <- 10
		3:
			p <- 3
		4:
			p <- 2
		5:
			p <- 7
		6:
			p <- 7
	FinSegun
FinFuncion

// ---------- BÚSQUEDA (binaria, O(log n)) ----------
Funcion pos <- BuscarExpediente(codigo, n, buscado)
	Definir pos, inf, sup, medio Como Entero
	pos <- 0
	inf <- 1
	sup <- n
	Mientras inf <= sup Y pos = 0 Hacer
		medio <- trunc((inf + sup) / 2)
		Si codigo[medio] = buscado Entonces
			pos <- medio
		Sino
			Si codigo[medio] < buscado Entonces
				inf <- medio + 1
			Sino
				sup <- medio - 1
			FinSi
		FinSi
	FinMientras
FinFuncion

SubProceso BuscarYMostrar(codigo, fecha, remitente, asunto, area, plazo, estado, n)
	Definir buscado, pos Como Entero
	Escribir "Código a buscar: "
	Leer buscado
	pos <- BuscarExpediente(codigo, n, buscado)
	Si pos = 0 Entonces
		Escribir "Expediente no encontrado."
	Sino
		MostrarExpediente(codigo, fecha, remitente, asunto, area, plazo, estado, pos)
	FinSi
FinSubProceso

SubProceso MostrarExpediente(codigo, fecha, remitente, asunto, area, plazo, estado, p)
	Definir texto Como Caracter
	Segun estado[p] Hacer
		1:
			texto <- "Recibido"
		2:
			texto <- "En atención"
		3:
			texto <- "Archivado"
	FinSegun
	Escribir Sin Saltar codigo[p], " | ", fecha[p], " | ", remitente[p], " | ", asunto[p]
	Escribir " | ", area[p], " | plazo ", plazo[p], " días | ", texto
FinSubProceso

// ---------- ORDENAMIENTO (inserción estable por fecha, O(n^2)) ----------
SubProceso OrdenarPorFecha(fecha, orden, n)
	Definir i, j, clave Como Entero
	Definir seguir Como Logico
	Para i <- 1 Hasta n Hacer
		orden[i] <- i
	FinPara
	Para i <- 2 Hasta n Hacer
		clave <- orden[i]
		j <- i - 1
		seguir <- Verdadero
		Mientras seguir Hacer
			Si j < 1 Entonces
				seguir <- Falso
			Sino
				Si fecha[orden[j]] > fecha[clave] Entonces
					orden[j + 1] <- orden[j]
					j <- j - 1
				Sino
					seguir <- Falso
				FinSi
			FinSi
		FinMientras
		orden[j + 1] <- clave
	FinPara
FinSubProceso

SubProceso ListarPorFecha(codigo, fecha, remitente, asunto, area, plazo, estado, orden, n)
	Definir i Como Entero
	Si n = 0 Entonces
		Escribir "No hay expedientes registrados."
	Sino
		OrdenarPorFecha(fecha, orden, n)
		Para i <- 1 Hasta n Hacer
			MostrarExpediente(codigo, fecha, remitente, asunto, area, plazo, estado, orden[i])
		FinPara
	FinSi
FinSubProceso

SubProceso CambiarEstado(codigo, estado, n)
	Definir buscado, pos, nuevo Como Entero
	Escribir "Código del expediente: "
	Leer buscado
	pos <- BuscarExpediente(codigo, n, buscado)
	Si pos = 0 Entonces
		Escribir "Expediente no encontrado."
	Sino
		Repetir
			Escribir "Nuevo estado (1 Recibido, 2 En atención, 3 Archivado): "
			Leer nuevo
		Hasta Que nuevo >= 1 Y nuevo <= 3
		estado[pos] <- nuevo
		Escribir "Estado actualizado."
	FinSi
FinSubProceso
