----------------------------------------------------------------------------------------------------------
								/*CREACION DEL MODELO DE DATOS*/

----------------------------------------------------------------------------------------------------------
								/*BD GD2C2014*/
				
/*USE GD2C2014*/

----------------------------------------------------------------------------------------------------------
								/*CREACION DEL ESQUEMA*/

CREATE SCHEMA GESTION_DE_GATOS AUTHORIZATION gd
GO

----------------------------------------------------------------------------------------------------------
								/*CREACION DE LAS FUNCIONES*/

--CREACION FUNCIONES
CREATE FUNCTION GESTION_DE_GATOS.calcPrecioConsumible ( @estadia numeric(18,0))

	RETURNS numeric(18,2)

AS

BEGIN

	DECLARE @total numeric(18,0)

	set @total = (select CostosConsumibles.Gasto 
				from 
					(	select cxe.estadia Est, SUM(con.precio) Gasto
						from GESTION_DE_GATOS.ConsumibleXEstadia cxe,
								GESTION_DE_GATOS.Consumibles con
						where cxe.consumible = con.idConsumible
								group by cxe.estadia
						) as CostosConsumibles
				where CostosConsumibles.Est = @estadia
				)
	if(@total is null)
	begin
	set @total=0
	end
RETURN @total

END	
go

CREATE FUNCTION GESTION_DE_GATOS.calcPrecioEstadia ( @estadia numeric(18,0))

	RETURNS numeric(18,2)

AS

BEGIN

	DECLARE @total numeric(18,0)

	set @total = (select CostosEstadias.Gasto 
				from 
					( select es.idEstadia Est, SUM(re.costoTotal) Gasto
					from GESTION_DE_GATOS.Estadia es,
						GESTION_DE_GATOS.Reserva re
					where es.reserva = re.idReserva
							group by es.idEstadia) as CostosEstadias
				where CostosEstadias.Est = @estadia
				)
RETURN @total

END	
go

CREATE FUNCTION GESTION_DE_GATOS.calcEstadiaDiasSobran 
	( @finRes datetime, @salidaEs datetime)

	RETURNS numeric(18,0)

AS

BEGIN

	DECLARE @sobranDias numeric(18,0)

	set @sobranDias = DATEDIFF(day,@salidaEs,@finRes)

	RETURN @sobranDias

END
go
-------------------------------------------------------------
CREATE FUNCTION GESTION_DE_GATOS.concatenarNombreHotel 
	( @nombre nvarchar(255), @numero numeric(18,0))

	RETURNS nvarchar(255)

AS

BEGIN

	DECLARE @aux nvarchar(255)

	set @aux = CONVERT(nvarchar(255),@numero) 

	RETURN @nombre+' '+@aux

END
go
-------------------------------------------------------------
create function GESTION_DE_GATOS.TieneReserva
( @desde datetime, @hasta datetime,@id numeric(18,0))

	RETURNS bit
as
begin
-- caso donde la fecha de inicio de la reserva, esta entre las pedidas
if(exists 
	(select RH.habitacion	
	from GESTION_DE_GATOS.ReservaXHabitacion RH, GESTION_DE_GATOS.Reserva R
	where 
	R.fecha_inicio > @desde and	
	R.fecha_inicio < @hasta and
	R.idReserva = RH.reserva and 
	RH.habitacion = @id  and 
	R.idReserva not in (select reserva from GESTION_DE_GATOS.Cancelacion C) ) )
begin
return 1
end
-- caso donde la fecha hasta de la reserva, esta entre las pedidas
if(exists 
	(select RH.habitacion	
	from GESTION_DE_GATOS.ReservaXHabitacion RH, GESTION_DE_GATOS.Reserva R
	where 
	R.fecha_hasta > @desde and	
	R.fecha_hasta < @hasta and
	R.idReserva = RH.reserva and 
	RH.habitacion = @id  and 
	R.idReserva not in (select reserva from GESTION_DE_GATOS.Cancelacion C) ) )
begin
return 2
end
-- caso donde la fecha de inicio de la reserva pedida, esta entre las fechas de la reserva
if(exists 
	(select RH.habitacion	
	from GESTION_DE_GATOS.ReservaXHabitacion RH, GESTION_DE_GATOS.Reserva R
	where 
	R.fecha_inicio < @desde and	
	R.fecha_hasta > @desde and
	R.idReserva = RH.reserva and 
	RH.habitacion = @id  and 
	R.idReserva not in (select reserva from GESTION_DE_GATOS.Cancelacion C) ) )
begin
return 3
end
-- caso donde la fecha hasta de la reserva pedida, esta entre las fechas de la reserva
if(exists 
	(select RH.habitacion	
	from GESTION_DE_GATOS.ReservaXHabitacion RH, GESTION_DE_GATOS.Reserva R
	where 
	R.fecha_inicio < @hasta and	
	R.fecha_hasta > @hasta and
	R.idReserva = RH.reserva and 
	RH.habitacion = @id  and 
	R.idReserva not in (select reserva from GESTION_DE_GATOS.Cancelacion C) ) )
begin
return 4
end
-- caso donde las fechas son iguales
if(exists 
	(select RH.habitacion	
	from GESTION_DE_GATOS.ReservaXHabitacion RH, GESTION_DE_GATOS.Reserva R
	where 
	R.fecha_inicio = @desde and	
	R.fecha_hasta = @hasta and
	R.idReserva = RH.reserva and 
	RH.habitacion = @id  and 
	R.idReserva not in (select reserva from GESTION_DE_GATOS.Cancelacion C) ) )
begin
return 5
end

return 0

end
go
-------------------------------------------------------------
create function GESTION_DE_GATOS.hotelTieneReserva
( @desde datetime, @hasta datetime,@id numeric(18,0))
	RETURNS bit
as
begin

--Creo tabla
declare @t table(
fecha_inicio DateTime NOT NULL,
fecha_hasta DateTime NOT NULL
)

--Inserto datos
insert into @t(
	fecha_inicio,
	fecha_hasta
	)
select Distinct r.fecha_inicio, r.fecha_hasta
from GESTION_DE_GATOS.Reserva AS r,
 GESTION_DE_GATOS.Habitacion AS h,
 GESTION_DE_GATOS.Hotel AS o,
 GESTION_DE_GATOS.ReservaXHabitacion AS x
Where h.hotel=@id
AND x.habitacion = h.idHabitacion
AND x.reserva = r.idReserva and
r.idReserva not in(select reserva from GESTION_DE_GATOS.Cancelacion C);

--Devuelvo resultado

-- caso donde la fecha inicio de la baja se encuentra en un periodio reservado
if(exists 
	(select fecha_inicio fecha_hasta from @t
	where (
	(fecha_inicio < @desde and	
	fecha_hasta > @desde)
	or
	(fecha_inicio < @hasta and	
	fecha_hasta > @hasta)
	or
	(fecha_inicio > @desde and	
	fecha_hasta < @hasta)
	)))
begin
return 1
end

return 0

end
go

----------------------------------------------------------------------------------------------------------
								/*CREACION DE LOS STORED PROCEDURES*/

create proc GESTION_DE_GATOS.BuscarHotelDeUser
@rol NVARCHAR(45),
@usuario numeric(18,0)
as
select H.idHotel from GESTION_DE_GATOS.Hotel H, GESTION_DE_GATOS.UserXRolXHotel U, GESTION_DE_GATOS.Rol R
where
U.usuario = @usuario and
@rol = R.descripcion and
R.idRol = U.rol and
H.idHotel = U.hotel
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.EliminarReservasAnteriores

@fecha nvarchar(20)
as
begin
declare @hoy datetime
Set @hoy = CONVERT(datetime,@fecha,121)
declare @estado numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	
	set @estado=(select idEstado from GESTION_DE_GATOS.Estado where descripcion = 'RESERVA CANCELADA POR NO-SHOW')
	update GESTION_DE_GATOS.Reserva 
	set estado = @estado 
	where idReserva in (
	select distinct R.idReserva from GESTION_DE_GATOS.Reserva R
	where
	R.fecha_inicio < @hoy and
	R.idReserva not in (select distinct reserva from GESTION_DE_GATOS.Estadia))
	
	insert into GESTION_DE_GATOS.Cancelacion (usuario,reserva,motivo,dia)
	select distinct 1,idReserva,'Cancelada por no-show al inicio del sistema',fecha_inicio from GESTION_DE_GATOS.Reserva where
	estado = @estado
	set @respuesta = 1
	select @respuesta as respuesta

commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarDireccion
@calle nvarchar(255),
@numero numeric(18,0),
@piso numeric(18,0)=NULL,
@depto nvarchar(50)=NULL,
@ciudad nvarchar(255),
@paisP nvarchar(255) 
as
begin
declare @pais numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	set @pais =(select idPais from GESTION_DE_GATOS.Pais
	where
	@paisP=nombre)
	insert into GESTION_DE_GATOS.Direccion (calle,numero,piso,depto,ciudad,pais) values(@calle,@numero,@piso,@depto,@ciudad,@pais);
	if(@piso is null and @depto is null)
	begin
	set @respuesta=(select idDir from GESTION_DE_GATOS.Direccion where @calle=calle and numero=@numero and piso is null and depto is null and @ciudad=ciudad and @pais=pais)
	end
	if(@piso is not null and @depto is not null)
	begin
	set @respuesta=(select idDir from GESTION_DE_GATOS.Direccion where @calle=calle and numero=@numero and piso=@piso and depto=@depto and @ciudad=ciudad and @pais=pais)
	end
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.ModificarDireccion
@id numeric(18,0),
@calle nvarchar(255),
@numero numeric(18,0),
@piso numeric(18,0)=NULL,
@depto nvarchar(50)=NULL,
@ciudad nvarchar(255),
@paisP nvarchar(255) 
as
begin
declare @pais numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	set @pais =(select idPais from GESTION_DE_GATOS.Pais
	where
	@paisP=nombre)
	update GESTION_DE_GATOS.Direccion
	set calle = @calle,
	numero = @numero,
	piso = @piso,
	depto = @depto,
	ciudad = @ciudad,
	pais = @pais
	where 
	idDir = @id;
	if(@piso is null and @depto is null)
	begin
	set @respuesta=(select idDir from GESTION_DE_GATOS.Direccion where @calle=calle and numero=@numero and piso is null and depto is null and @ciudad=ciudad and @pais=pais)
	end
	if(@piso is not null and @depto is not null)
	begin
	set @respuesta=(select idDir from GESTION_DE_GATOS.Direccion where @calle=calle and numero=@numero and piso=@piso and depto=@depto and @ciudad=ciudad and @pais=pais)
	end
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarUser
@usuario nvarchar(255),
@clave nvarchar(255),
@rolR nvarchar(255),
@nombre nvarchar(255),
@apellido nvarchar(255),
@tel numeric(18,0),
@mail nvarchar(255),
@tipoDocT nvarchar(255),
@nroDoc numeric(18,0),
@hotelH nvarchar(255),
@fechanacI datetime,
@direccion numeric(18,0),
@estado bit

as
begin
declare @fechanac datetime
Set @fechanac = CONVERT(datetime,@fechanacI,121)
declare @tipoDoc numeric(18,0)
declare @rol numeric(18,0)
declare @hotel numeric(18,0)
declare @respuesta numeric(18,0)
declare @idUser numeric(18,0)
begin tran ta
begin try
	set @tipoDoc =(select idTipoDoc from GESTION_DE_GATOS.TiposDoc
	where
	@tipoDocT=descripcion)
	set @rol =(select idRol from GESTION_DE_GATOS.Rol
	where
	@rolR=descripcion)
	set @hotel =(select idHotel from GESTION_DE_GATOS.Hotel
	where
	@hotelH=nombre)
	if( not exists (select userName from GESTION_DE_GATOS.Usuario where userName=@usuario) )
	begin
		insert into GESTION_DE_GATOS.Usuario (userName,clave,nombre,apellido,tipoDoc,nroDoc,fecha_nac,mail,telefono,estado,direccion)
		values(@usuario,@clave,@nombre,@apellido,@tipoDoc,@nroDoc,@fechanac,@mail,@tel,@estado,@direccion);
	end
	set @idUser = (select idUsuario from GESTION_DE_GATOS.Usuario where @usuario = userName);
	insert into GESTION_DE_GATOS.UserXRolXHotel (usuario,rol,hotel)
	values(@idUser,@rol,@hotel);
	set @respuesta = 1;
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
CREATE proc [GESTION_DE_GATOS].[ModificarUser]
@id NUMERIC(18,0),
@idUR NUMERIC(18,0),
@usuario nvarchar(255),
@clave nvarchar(255),
@rolR nvarchar(255),
@nombre nvarchar(255),
@apellido nvarchar(255),
@tel numeric(18,0),
@mail nvarchar(255),
@tipoDocT nvarchar(255),
@nroDoc numeric(18,0),
@hotelH nvarchar(255),
@fechanacI datetime,
@direccion numeric(18,0),
@estado bit

as
begin
declare @fechanac datetime
Set @fechanac = CONVERT(datetime,@fechanacI,121)
declare @tipoDoc numeric(18,0)
declare @rol numeric(18,0)
declare @hotel numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	set @tipoDoc =(select idTipoDoc from GESTION_DE_GATOS.TiposDoc
	where
	@tipoDocT=descripcion);
	set @rol =(select idRol from GESTION_DE_GATOS.Rol
	where
	@rolR=descripcion);
	set @hotel =(select idHotel from GESTION_DE_GATOS.Hotel
	where
	@hotelH=nombre);
	
	UPDATE GESTION_DE_GATOS.Usuario
	
	set userName = @usuario,
	clave = @clave,
	nombre = @nombre,
	apellido = @apellido,
	tipoDoc = @tipoDoc,
	nroDoc = @nroDoc,
	fecha_nac = @fechanac,
	mail = @mail,
	telefono =@tel,
	estado = @estado,
	direccion = @direccion
	where idUsuario = @id;
	
	update GESTION_DE_GATOS.UserXRolXHotel
	set rol=@rol, 
	hotel = @hotel
	where usuario = @id and idUserXRol = @idUR;
	
	set @respuesta = 1;
	select @respuesta as respuesta;
	
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarHabitacion
@numero numeric(18,0),
@piso numeric(18,0),
@ubicacion nvarchar(20)=NULL,
@tipoT nvarchar(255),
@hotel numeric(18,0),
@descripcion nvarchar(100)=NULL ,
@estado bit
as
begin
declare @tipo numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	set @tipo =(select codigo from GESTION_DE_GATOS.TipoHabitacion
	where
	@tipoT=descripcion)
	insert into GESTION_DE_GATOS.Habitacion (numero,piso,ubicacion,tipo,hotel,descripcion,estado) 
	values(@numero,@piso,@ubicacion,@tipo,@hotel,@descripcion,@estado);
	
	set @respuesta=(select idHabitacion from GESTION_DE_GATOS.Habitacion where @numero=numero and piso=@piso and tipo=@tipo and @hotel=hotel)

	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.ModificarHabitacion
@id numeric(18,0),
@numero numeric(18,0),
@piso numeric(18,0),
@ubicacion nvarchar(20)=NULL,
@tipoT nvarchar(255),
@hotel numeric(18,0),
@descripcion nvarchar(100)=NULL ,
@estado bit
as
begin
declare @tipo numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	set @tipo =(select codigo from GESTION_DE_GATOS.TipoHabitacion
	where
	@tipoT=descripcion)
	update GESTION_DE_GATOS.Habitacion
	set numero = @numero,
	piso = @piso,
	ubicacion = @ubicacion,
	tipo = @tipo,
	hotel = @hotel,
	descripcion = @descripcion,
	estado = @estado
	where
	idHabitacion = @id
	
	set @respuesta=(select idHabitacion from GESTION_DE_GATOS.Habitacion where idHabitacion = @id)

	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarFuncXRol
@rol numeric(18,0),
@funcionalidad numeric(18,0)
as
begin
declare @respuesta numeric(18,0)
begin tran ta
begin try
	insert into GESTION_DE_GATOS.FuncXRol (rol,funcionalidad) values(@rol,@funcionalidad);
	set @respuesta =(select idFuncXRol from GESTION_DE_GATOS.FuncXRol where rol = @rol and funcionalidad = @funcionalidad)
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarRol
@rol nvarchar(45),
@estado bit
as
begin
declare @respuesta numeric(18,0)
begin tran ta
begin try
	insert into GESTION_DE_GATOS.Rol (descripcion,estado) values(@rol,@estado);
	set @respuesta =(select idRol from GESTION_DE_GATOS.Rol where descripcion = @rol)
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarCliente
@nombre nvarchar(255),
@apellido nvarchar(255),
@tel numeric(18,0),
@mail nvarchar(255),
@tipoDocT nvarchar(255),
@nroDoc numeric(18,0),
@fechanacI datetime,
@direccion numeric(18,0),
@nacionalidadN nvarchar(255),
@estado bit

as
begin
declare @fechanac datetime
Set @fechanac = CONVERT(datetime,@fechanacI,121)
declare @tipoDoc numeric(18,0)
declare @nacionalidad numeric(18,0)
declare @respuesta numeric(18,0)
declare @idCli numeric(18,0)
begin tran ta
begin try
	set @tipoDoc =(select idTipoDoc from GESTION_DE_GATOS.TiposDoc
	where
	@tipoDocT=descripcion)
	set @nacionalidad =(select idPais from GESTION_DE_GATOS.Pais
	where
	@nacionalidadN=nombre)
	if( not exists (select tipoDoc,nroDoc from GESTION_DE_GATOS.Cliente where tipoDoc = @tipoDoc and nroDoc = @nroDoc) )
	begin
		insert into GESTION_DE_GATOS.Cliente (nombre,apellido,tipoDoc,nroDoc,fecha_nac,mail,telefono,habilitado,direccion,nacionalidad)
		values(@nombre,@apellido,@tipoDoc,@nroDoc,@fechanac,@mail,@tel,@estado,@direccion,@nacionalidad);
		set @respuesta = 1;
	end
	else
	begin
	set @respuesta = 2;
	end
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
CREATE proc [GESTION_DE_GATOS].[ModificarCliente]
@id NUMERIC(18,0),
@nombre nvarchar(255),
@apellido nvarchar(255),
@tel numeric(18,0),
@mail nvarchar(255),
@tipoDocT nvarchar(255),
@nroDoc numeric(18,0),
@fechanacI datetime,
@direccion numeric(18,0),
@nacionalidadN nvarchar(255),
@estado bit

as
begin
declare @fechanac datetime
Set @fechanac = CONVERT(datetime,@fechanacI,121)
declare @tipoDoc numeric(18,0)
declare @nacionalidad numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	set @tipoDoc =(select idTipoDoc from GESTION_DE_GATOS.TiposDoc
	where
	@tipoDocT=descripcion)
	set @nacionalidad =(select idPais from GESTION_DE_GATOS.Pais
	where
	@nacionalidadN=nombre)
	
	UPDATE GESTION_DE_GATOS.Cliente
	
	set nombre = @nombre,
	apellido = @apellido,
	tipoDoc = @tipoDoc,
	nroDoc = @nroDoc,
	fecha_nac = @fechanac,
	mail = @mail,
	telefono =@tel,
	habilitado = @estado,
	direccion = @direccion,
	nacionalidad = @nacionalidad
	where idCli = @id;
		
	set @respuesta = 1;
	select @respuesta as respuesta;
	
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.BuscarTiposHabdeHotel
@nombreHotel nvarchar(255)
as
begin
select distinct T.descripcion from GESTION_DE_GATOS.Hotel HO, GESTION_DE_GATOS.TipoHabitacion T, GESTION_DE_GATOS.Habitacion H
where
HO.nombre = @nombreHotel AND
H.hotel = HO.idHotel and
T.codigo = H.tipo
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.BuscarRegimenesDeHotel
@nombreHotel nvarchar(255)
as
begin
select distinct R.descripcion from GESTION_DE_GATOS.Hotel H, GESTION_DE_GATOS.RegimenXHotel RH, GESTION_DE_GATOS.Regimen R
where
H.nombre = @nombreHotel AND
RH.hotel = H.idHotel and
RH.regimen = R.codigo
end
GO
-------------------------------------------------------------
create proc GESTION_DE_GATOS.BuscarHabitacionDisponibles

@nombreHo nvarchar(255),
@fechaDesdeD datetime,
@fechaHastaH datetime,
@regimen nvarchar(255) = null,
@tipoHa nvarchar(255)

as

begin
declare @fechaDesde datetime
Set @fechaDesde = CONVERT(datetime,@fechaDesdeD,121)
declare @fechaHasta datetime
Set @fechaHasta = CONVERT(datetime,@fechaHastaH,121)
declare @hotel numeric(18,0)
set @hotel = (select idHotel from GESTION_DE_GATOS.Hotel where nombre = @nombreHo)

declare @tipo numeric(18,0)
set @tipo = (select codigo from GESTION_DE_GATOS.TipoHabitacion where descripcion = @tipoHa)
if(@regimen is null)
begin
	select distinct Ha.idHabitacion,Ha.ubicacion, (R.precio*T.cantPersonas)+(Ho.cant_estrellas*Ho.recarga_estrella) PrecioXNoche ,R.descripcion
	from GESTION_DE_GATOS.Habitacion Ha, GESTION_DE_GATOS.Hotel Ho, GESTION_DE_GATOS.TipoHabitacion T,GESTION_DE_GATOS.Regimen R
	where
	Ho.idHotel = @hotel and
	T.codigo = @tipo and
	Ha.hotel = @hotel and
	Ha.tipo = @tipo and
	not exists (select hotel from GESTION_DE_GATOS.BajaHotel where hotel=@hotel and fecha_ini<=@fechaDesde and fecha_hasta>=@fechaDesde)
	and 
	not exists (select hotel from GESTION_DE_GATOS.BajaHotel where hotel=@hotel and fecha_ini<=@fechaHasta and fecha_hasta>=@fechaHasta)
	and
	(GESTION_DE_GATOS.TieneReserva(@fechaDesde,@fechaHasta,Ha.idHabitacion)) <1
end
else
begin
	declare @regi numeric(18,0)
	set @regi = (select codigo from GESTION_DE_GATOS.Regimen where descripcion = @regimen)
	select distinct Ha.idHabitacion,Ha.ubicacion, (R.precio*T.cantPersonas)+(Ho.cant_estrellas*Ho.recarga_estrella) PrecioXNoche ,R.descripcion
	from GESTION_DE_GATOS.Habitacion Ha, GESTION_DE_GATOS.Hotel Ho, GESTION_DE_GATOS.TipoHabitacion T,GESTION_DE_GATOS.Regimen R
	where
	R.codigo = @regi and
	Ho.idHotel = @hotel and
	T.codigo = @tipo and
	Ha.hotel = @hotel and
	Ha.tipo = @tipo and
	not exists (select hotel from GESTION_DE_GATOS.BajaHotel where hotel=@hotel and fecha_ini<=@fechaDesde and fecha_hasta>=@fechaDesde)
	and 
	not exists (select hotel from GESTION_DE_GATOS.BajaHotel where hotel=@hotel and fecha_ini<=@fechaHasta and fecha_hasta>=@fechaHasta)
	and
	(GESTION_DE_GATOS.TieneReserva(@fechaDesde,@fechaHasta,Ha.idHabitacion)) <1
end

end
go
----------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarReserva
@fechaIniI datetime,
@regimenR nvarchar(255),
@fechaHastaI datetime,
@fechaRegI datetime,
@usuario numeric(18,0),
@cliente numeric(18,0),
@cantNoches numeric(18,0),
@costoTotal numeric(18,0)

as

begin
declare @fechaIni datetime
Set @fechaIni = CONVERT(datetime,@fechaIniI,121)
declare @fechaHasta datetime
Set @fechaHasta = CONVERT(datetime,@fechaHastaI,121)
declare @fechaReg datetime
Set @fechaReg = CONVERT(datetime,@fechaRegI,121)

declare @regimen numeric(18,0)
declare @respuesta numeric(18,0)
declare @estado numeric(18,0)

begin tran ta
begin try
	set @regimen =(select codigo from GESTION_DE_GATOS.Regimen
	where
	@regimenR=descripcion)
	set @estado =(select idEstado from GESTION_DE_GATOS.Estado
	where
	'RESERVA CORRECTA'=descripcion)
	
	insert into GESTION_DE_GATOS.Reserva (fecha_inicio,regimen,fecha_hasta,fecha_registro,estado,usuario,cliente,cantNoches,costoTotal)
	values(@fechaIni,@regimen,@fechaHasta,@fechaReg,@estado,@usuario,@cliente,@cantNoches,@costoTotal);
	
	set @respuesta = (select SCOPE_IDENTITY());
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarReservaXHabitacion
@reserva numeric(18,0),
@habitacion numeric(18,0)
as
begin
declare @respuesta numeric(18,0)
begin tran ta
begin try
	insert into GESTION_DE_GATOS.ReservaXHabitacion (reserva,habitacion) values(@reserva,@habitacion);
	set @respuesta =(select idReservaXHabitacion from GESTION_DE_GATOS.ReservaXHabitacion where reserva = @reserva and habitacion = @habitacion)
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.ModificarReserva
@idRes numeric(18,0),
@fechaIniI datetime,
@regimenR nvarchar(255),
@fechaHastaI datetime,
@fechaRegI datetime,
@usuario numeric(18,0),
@cantNoches numeric(18,0),
@costoTotal numeric(18,0)

as

begin
declare @fechaIni datetime
Set @fechaIni = CONVERT(datetime,@fechaIniI,121)
declare @fechaHasta datetime
Set @fechaHasta = CONVERT(datetime,@fechaHastaI,121)
declare @fechaReg datetime
Set @fechaReg = CONVERT(datetime,@fechaRegI,121)
declare @regimen numeric(18,0)
declare @respuesta numeric(18,0)
declare @estado numeric(18,0)

begin tran ta
begin try
	set @regimen =(select codigo from GESTION_DE_GATOS.Regimen
	where
	@regimenR=descripcion)
	set @estado =(select idEstado from GESTION_DE_GATOS.Estado
	where
	'RESERVA MODIFICADA'=descripcion)
	
	UPDATE GESTION_DE_GATOS.Reserva 
	set fecha_inicio = @fechaIni,
	regimen = @regimen,
	fecha_hasta =@fechaHasta ,
	estado = @estado,
	usuario = @usuario, 
	cantNoches = @cantNoches,
	costoTotal = @costoTotal
	WHERE
	idReserva = @idRes

	insert into GESTION_DE_GATOS.Modificacion (usuario,reserva,dia)
	values(@usuario,@idRes,@fechaReg)
	
	set @respuesta = 1;
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.EliminarHabitacionesDeReserva
@reserva numeric(18,0),
@habitacion numeric(18,0)
as
begin
declare @respuesta numeric(18,0)
begin tran ta
begin try
	delete from GESTION_DE_GATOS.ReservaXHabitacion where reserva = @reserva and habitacion = @habitacion;
	set @respuesta =1
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.BuscarHabitacionDeReserva
@reserva numeric(18,0)
as
begin
select distinct RH.habitacion,(RE.precio * T.cantPersonas) + (HO.cant_estrellas * HO.recarga_estrella)
from GESTION_DE_GATOS.Reserva R,  GESTION_DE_GATOS.Regimen RE, GESTION_DE_GATOS.Habitacion H,
	GESTION_DE_GATOS.Hotel HO,
	GESTION_DE_GATOS.ReservaXHabitacion RH,
	GESTION_DE_GATOS.TipoHabitacion T
where
R.idReserva = @reserva and
R.regimen = RE.codigo and
RH.reserva = @reserva and
RH.habitacion = H.idHabitacion and
H.hotel = HO.idHotel AND
H.tipo = T.codigo

end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.CancelarReserva
@reserva numeric(18,0),
@estado numeric(18,0),
@usuario numeric(18,0),
@fechaI datetime,
@motivo nvarchar(255)
as
begin
declare @fecha datetime
Set @fecha = CONVERT(datetime,@fechaI,121)

declare @respuesta numeric(18,0)
begin tran ta
begin try
	update GESTION_DE_GATOS.Reserva 
	set estado = @estado
	where idReserva = @reserva;
	
	insert into GESTION_DE_GATOS.Cancelacion(usuario,reserva,motivo,dia)
	values(@usuario,@reserva,@motivo,@fecha)
	
	set @respuesta =1
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------

create proc GESTION_DE_GATOS.registrarCheckinEstadia
@reservaNro numeric(18,0),
@userNro numeric(18,0),
@fechaI datetime
as
begin

declare @fecha datetime
Set @fecha = CONVERT(datetime,@fechaI,121)
declare @respuesta numeric(18,0),
		@precioNoche numeric(18,0),
		@cantNoches numeric(18,0),
		@estado numeric(18,0)
begin tran ta
begin try
	set @estado = (select idEstado from GESTION_DE_GATOS.Estado where descripcion = 'RESERVA EFECTIVIZADA')
	set @cantNoches = (select cantNoches from GESTION_DE_GATOS.Reserva  where idReserva = @reservaNro)
	set @precioNoche = (select costoTotal from GESTION_DE_GATOS.Reserva res where res.idReserva = @reservaNro)/@cantNoches
	insert into GESTION_DE_GATOS.Estadia(reserva,ingreso,usuario,precioPorNoche,cantidadNoches) values(@reservaNro,@fecha,@userNro,@precioNoche,@cantNoches);
	update GESTION_DE_GATOS.Reserva set estado=@estado where idReserva = @reservaNro
	set @respuesta =(select idEstadia from GESTION_DE_GATOS.Estadia where reserva = @reservaNro and ingreso=@fecha)
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO

--------------------------------------------------------------

create procedure GESTION_DE_GATOS.RegistrarEstadiaXCliente
@clienteNro numeric(18,0),
@estadiaNro numeric(18,0)
as 
begin
declare @respuesta numeric(18,0)
begin tran ta
begin try
	insert into GESTION_DE_GATOS.ClienteXEstadia (cliente,estadia) values(@clienteNro,@estadiaNro);
	set @respuesta = (select idClienteXEstadia from GESTION_DE_GATOS.ClienteXEstadia where estadia = @estadiaNro and cliente = @clienteNro)
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO

--------------------------------------------------------------

create proc GESTION_DE_GATOS.ModificarClienteXEstadia
@habitacion numeric(18,0),
@cliente numeric(18,0),
@estadia numeric(18,0)
as
begin

declare @respuesta numeric(18,0)
begin tran ta
begin try
	
	update GESTION_DE_GATOS.ClienteXEstadia
	set habitacion = @habitacion
	where estadia = @estadia and cliente = @cliente
	set @respuesta = 1
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.registrarCheckoutEstadia
@idEstadia numeric(18,0),
@fechaI datetime,
@usuario numeric(18,0)
as
begin

declare @fecha datetime
Set @fecha = CONVERT(datetime,@fechaI,121)
declare @respuesta numeric(18,0),
		@cantDias numeric(18,0),
		@diasSobran numeric(18,0),
		@precioNoche numeric(18,0),
		@nroReserva numeric(18,0),
		@resHasta datetime,
		@factu numeric(18,0),
		@monto numeric(18,0)
begin tran ta
begin try
	set @cantDias = DATEDIFF(day,( select ingreso from GESTION_DE_GATOS.Estadia where idEstadia = @idEstadia ),@fecha);
	set @nroReserva = (select reserva from GESTION_DE_GATOS.Estadia where idEstadia = @idEstadia);
	set @resHasta = (select fecha_hasta from GESTION_DE_GATOS.Reserva where idReserva = @nroReserva);
	set @diasSobran = DATEDIFF(day,@fecha,@resHasta);
	
	UPDATE GESTION_DE_GATOS.Estadia
	set 
		usuarioSalida = @usuario,
		salida = @fecha,
		dias_sobran = @diasSobran,
		cantidadNoches =  @cantDias
		where idEstadia = @idEstadia;
	
	set @monto = (select costoTotal from GESTION_DE_GATOS.Reserva where idReserva =@nroReserva)
	set @factu = (select numero from GESTION_DE_GATOS.Factura where estadia = @idEstadia)
	insert into GESTION_DE_GATOS.ItemFactura(factura,descripcion,cantidad,monto)
	values(@factu,'Estadia',1, @monto)
	set @respuesta = 1;
	select @respuesta as respuesta;
	
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create procedure GESTION_DE_GATOS.generarFactura 
@estadia numeric(18,0),
@formaPago numeric(18,0),
@fechaI datetime
as
begin

declare @fecha datetime
Set @fecha = CONVERT(datetime,@fechaI,121)
declare @resultado numeric(18,0),
		@regimen numeric(18,0),
		@reserva numeric(18,0),
		@totalPago numeric(18,2),
		@cliente numeric(18,0)
begin tran ta
begin try
		set @reserva = ( select reserva from GESTION_DE_GATOS.Estadia where idEstadia = @estadia);
		set @cliente = ( select Cliente from GESTION_DE_GATOS.Reserva where idReserva = @reserva);
		set @regimen = ( select regimen from GESTION_DE_GATOS.Reserva where idReserva = @reserva);
		if ( @regimen != 3 )
		begin
			set @totalPago = GESTION_DE_GATOS.calcPrecioEstadia(@estadia) + GESTION_DE_GATOS.calcPrecioConsumible(@estadia);
			insert into GESTION_DE_GATOS.Factura(estadia,formaDePago,cliente,fecha,total) values (@estadia,@formaPago,@cliente,@fecha,@totalPago);
		end
		else
			begin
				set @totalPago = GESTION_DE_GATOS.calcPrecioEstadia(@estadia);
				insert into GESTION_DE_GATOS.Factura(estadia,formaDePago,cliente,fecha,total) values (@estadia,@formaPago,@cliente,@fecha,@totalPago);
			end
		set @resultado =(select numero from GESTION_DE_GATOS.Factura where estadia = @estadia and fecha = @fecha)
		select @resultado as resultado
commit tran	ta
end try
begin catch
rollback tran ta
set @resultado=0
select @resultado as resultado
end catch
end
GO
--------------------------------------------------------------
create procedure GESTION_DE_GATOS.ModificarFactura 
@estadia numeric(18,0),
@formaPago numeric(18,0),
@fechaI datetime
as
begin

declare @fecha datetime
Set @fecha = CONVERT(datetime,@fechaI,121)
declare @resultado numeric(18,0),
		@regimen numeric(18,0),
		@reserva numeric(18,0),
		@totalPago numeric(18,2),
		@cliente numeric(18,0),
		@factu numeric(18,0)
begin tran ta
begin try
		set @reserva = ( select reserva from GESTION_DE_GATOS.Estadia where idEstadia = @estadia);
		set @cliente = ( select cliente from GESTION_DE_GATOS.Reserva where idReserva = @reserva);
		set @regimen = ( select regimen from GESTION_DE_GATOS.Reserva where idReserva = @reserva);
		set @factu = (select numero from GESTION_DE_GATOS.Factura where estadia = @estadia);
		if ( @regimen != 3 )
		begin
			set @totalPago = GESTION_DE_GATOS.calcPrecioEstadia(@estadia) + GESTION_DE_GATOS.calcPrecioConsumible(@estadia);
			update GESTION_DE_GATOS.Factura
			set formaDePago =@formaPago ,
			fecha = @fecha,
			total = @totalPago
			where
			numero = @factu
		end
		else
		begin
			set @totalPago = GESTION_DE_GATOS.calcPrecioEstadia(@estadia);
			update GESTION_DE_GATOS.Factura
			set formaDePago =@formaPago ,
			fecha = @fecha,
			total = @totalPago
			where
			numero = @factu
		end
		set @resultado = (select numero from GESTION_DE_GATOS.Factura where estadia = @estadia and fecha = @fecha)
		select @resultado as resultado
commit tran	ta
end try
begin catch
rollback tran ta
set @resultado=0
select @resultado as resultado
end catch
end
GO
-------------------------------------------------
create procedure GESTION_DE_GATOS.registrarTarjeta 
	@factura numeric(18,0),
	@entidad nvarchar(100),
	@numero numeric(18,0),
	@banco nvarchar(100) = NULL,
	@titular nvarchar(100)
	as
	begin
		declare @resultado numeric(18,0)
	begin tran ta
		begin try
			insert into GESTION_DE_GATOS.Tarjeta(entidad,numero,banco,titular) values (@entidad,@numero,@banco,@titular)
			update GESTION_DE_GATOS.Factura set tarjeta = (select SCOPE_IDENTITY()) where numero = @factura
			set @resultado = 1;
			select @resultado as resultado;
			commit tran ta
		end try
	begin catch
	rollback tran ta
	set @resultado = 0
	select @resultado as resultado
	end catch
end
GO
--------------------------------------------------------------
create procedure GESTION_DE_GATOS.RegistrarConsXEstadiaXHab
@estadia numeric(18,0),
@cons numeric(18,0),
@hab numeric(18,0)
as
begin
declare @respuesta numeric(18,0)
declare @item numeric(18,0)
declare @descri nvarchar(255)
declare @factu numeric(18,0)
declare @precio numeric(18,0)
begin tran ta
begin try
		set @precio = (select precio from GESTION_DE_GATOS.Consumibles where idConsumible = @cons)
		set @descri = (	select descripcion from GESTION_DE_GATOS.Consumibles where idConsumible = @cons)
		set @factu = (select numero from GESTION_DE_GATOS.Factura where estadia = @estadia)
		if( exists(select id from GESTION_DE_GATOS.ItemFactura where descripcion = @descri and factura = @factu))
		begin
		set @item = (select id from GESTION_DE_GATOS.ItemFactura where descripcion = @descri and factura = @factu)
		update GESTION_DE_GATOS.ItemFactura 
		set monto = monto + @precio,
		cantidad = cantidad + 1
		where id=@item
		insert into GESTION_DE_GATOS.ConsumibleXEstadia (estadia,consumible,habitacion,item) values (@estadia,@cons,@hab,@item)
		end
		else
		begin
		insert into GESTION_DE_GATOS.ItemFactura(factura,cantidad,monto,descripcion)
		values(@factu,1,@precio,@descri)
		set @item = (select id from GESTION_DE_GATOS.ItemFactura where descripcion = @descri and factura = @factu)
		insert into GESTION_DE_GATOS.ConsumibleXEstadia (estadia,consumible,habitacion,item) values (@estadia,@cons,@hab,@item)
		end
		set @respuesta = 1
		select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta = 0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
Create procedure GESTION_DE_GATOS.listaMaximosPuntajes
				@trimestre numeric(18,0), @Anio numeric(18,0) as
				
				begin
		
		declare @inicio datetime
		declare @fin datetime
		declare @AnioAux char(4)
		set @AnioAux = CAST(@Anio as CHAR(4))
		
		if (@trimestre = 1)
			begin
			set @inicio = '01-01-'+@AnioAux
			set @fin = '31-03-'+@AnioAux
			end
			else if (@trimestre = 2)
				begin
				set @inicio = '01-04-'+@AnioAux
				set @fin = '30-06-'+@AnioAux
				end
				else if (@trimestre = 3)
					begin 
					set @inicio = '01-07-'+@AnioAux
					set @fin = '30-09-'+@AnioAux
					end
					else if (@trimestre = 4)
						begin
						set @inicio = '01-10-'+@AnioAux
						set @fin = '31-12-'+@AnioAux
						end
						else 
							begin
							set @inicio = '01-01-'+@AnioAux
							set @fin = '31-12-'+@AnioAux
							end


select top 5 cli.idCli, cli.nombre Nombre, cli.apellido Apellido, punEs.Puntos+punCon.Puntos Puntaje
from
(	select es.idEstadia, SUM(re.costoTotal) Gasto, SUM(re.costoTotal)/10 Puntos
	from GESTION_DE_GATOS.Estadia es,
			GESTION_DE_GATOS.Reserva re,
			GESTION_DE_GATOS.Factura fc
	where es.reserva = re.idReserva and
			fc.estadia = es.idEstadia and
			fc.fecha between @inicio and @fin
	group by es.idEstadia
	) as punEs, 

(	select cxe.estadia, SUM(con.precio) Gasto, SUM(con.precio)/5 Puntos
	from GESTION_DE_GATOS.ConsumibleXEstadia cxe,
			GESTION_DE_GATOS.Consumibles con,
			GESTION_DE_GATOS.Factura fc
	where cxe.consumible = con.idConsumible and
			fc.estadia = cxe.estadia and
			fc.fecha between @inicio and @fin
	group by cxe.estadia
	) as punCon,
	
GESTION_DE_GATOS.Cliente cli,
GESTION_DE_GATOS.Estadia es,
GESTION_DE_GATOS.Reserva res
where cli.idCli = res.cliente and 
		es.reserva = res.idReserva and 
		punCon.estadia = es.idEstadia and 
		punEs.idEstadia = es.idEstadia
order by Puntaje desc
end

GO
--------------------------------------------------------------
Create procedure GESTION_DE_GATOS.listaHabitacionesVecesOcupada
				@trimestre numeric(18,0), @Anio numeric(18,0) as
				
				begin
		
		declare @inicio datetime
		declare @fin datetime
		declare @AnioAux char(4)
		set @AnioAux = CAST(@Anio as CHAR(4))
		
		if (@trimestre = 1)
			begin
			set @inicio = '01-01-'+@AnioAux
			set @fin = '31-03-'+@AnioAux
			end
			else if (@trimestre = 2)
				begin
				set @inicio = '01-04-'+@AnioAux
				set @fin = '30-06-'+@AnioAux
				end
				else if (@trimestre = 3)
					begin 
					set @inicio = '01-07-'+@AnioAux
					set @fin = '30-09-'+@AnioAux
					end
					else if (@trimestre = 4)
						begin
						set @inicio = '01-10-'+@AnioAux
						set @fin = '31-12-'+@AnioAux
						end
						else 
							begin
							set @inicio = '01-01-'+@AnioAux
							set @fin = '31-12-'+@AnioAux
							end



select distinct top 5 ho.nombre Hotel_Nombre,
		hab.numero NumeroHab,
		hab.piso PisoHab,
		consulCantXHab.cantEst VecesOcupada,
		consultaCantDias.Dias cantDias
from				(

					select  hab.idHabitacion,
							hab.hotel,
							COUNT(hab.idHabitacion) cantEst
					from 
							GESTION_DE_GATOS.ReservaXHabitacion rxh,
							GESTION_DE_GATOS.Habitacion hab,
							GESTION_DE_GATOS.Reserva res,
							GESTION_DE_GATOS.Hotel ho,
							GESTION_DE_GATOS.Estadia es
					where rxh.habitacion = hab.idHabitacion and
						  rxh.reserva = res.idReserva and
						  ho.idHotel = hab.hotel and
						  es.reserva = res.idReserva
					group by hab.idHabitacion, hab.hotel) as consulCantXHab,
					
					(
						select  hab.idHabitacion,
								hab.hotel,
								SUM(CAST(es.salida-es.ingreso as numeric(18,0))) Dias
						from 
								GESTION_DE_GATOS.ReservaXHabitacion rxh,
								GESTION_DE_GATOS.Habitacion hab,
								GESTION_DE_GATOS.Reserva res,
								GESTION_DE_GATOS.Hotel ho,
								GESTION_DE_GATOS.Estadia es
						where rxh.habitacion = hab.idHabitacion and
							  rxh.reserva = res.idReserva and
							  ho.idHotel = hab.hotel and
							  es.reserva = res.idReserva
						group by hab.idHabitacion,hab.hotel) as consultaCantDias,

				GESTION_DE_GATOS.Hotel ho,
				GESTION_DE_GATOS.Habitacion hab,
				GESTION_DE_GATOS.Estadia es
where consulCantXHab.hotel = ho.idHotel and
		hab.idHabitacion = consulCantXHab.idHabitacion and
		consulCantXHab.idHabitacion = consultaCantDias.idHabitacion	and
		es.ingreso between @inicio and @fin
order by cantEst desc
	
	end
GO
--------------------------------------------------------------
Create procedure GESTION_DE_GATOS.lista_hoteles_maxResCancel
				@trimestre numeric(18,0), @Anio numeric(18,0) as
		
		begin
		
		declare @inicio datetime
		declare @fin datetime
		declare @AnioAux char(4)
		set @AnioAux = CAST(@Anio as CHAR(4))
		
		if (@trimestre = 1)
			begin
			set @inicio = '01-01-'+@AnioAux
			set @fin = '31-03-'+@AnioAux
			end
			else if (@trimestre = 2)
				begin
				set @inicio = '01-04-'+@AnioAux
				set @fin = '30-06-'+@AnioAux
				end
				else if (@trimestre = 3)
					begin 
					set @inicio = '01-07-'+@AnioAux
					set @fin = '30-09-'+@AnioAux
					end
					else if (@trimestre = 4)
						begin
						set @inicio = '01-10-'+@AnioAux
						set @fin = '31-12-'+@AnioAux
						end
						else 
							begin
							set @inicio = '01-01-'+@AnioAux
							set @fin = '31-12-'+@AnioAux
							end
							
		select top 5
		hot.nombre,
		count(res.estado) cancelaciones
		from GESTION_DE_GATOS.Reserva res,
			GESTION_DE_GATOS.Hotel hot,
			GESTION_DE_GATOS.Habitacion hab,
			GESTION_DE_GATOS.ReservaXHabitacion rxh,
			GESTION_DE_GATOS.Estado es,
			GESTION_DE_GATOS.Cancelacion can
		where res.estado = es.idEstado and (
		  es.descripcion = 'RESERVA CANCELADA POR RECEPCION' or
		  es.descripcion = 'RESERVA CANCELADA POR CLIENTE' or
		  es.descripcion = 'RESERVA CANCELADA POR NO-SHOW' or
		  es.descripcion = 'RESERVA CANCELADA DESDE TABLA MAESTRA'
		  ) and
		  hab.idHabitacion = rxh.habitacion and
		  hab.hotel = hot.idHotel and
		  can.reserva = res.idReserva and
		  rxh.reserva = res.idReserva and
		  can.dia between @inicio and @fin
		group by hot.nombre
		order by cancelaciones desc					
		end
GO
--------------------------------------------------------------
Create procedure GESTION_DE_GATOS.lista_Hotel_DiasFueraServ
				@trimestre numeric(18,0), @Anio numeric(18,0) as
		
		begin
		
		declare @inicio datetime
		declare @fin datetime
		declare @AnioAux char(4)
		set @AnioAux = CAST(@Anio as CHAR(4))
		
		if (@trimestre = 1)
			begin
			set @inicio = '01-01-'+@AnioAux
			set @fin = '31-03-'+@AnioAux
			end
			else if (@trimestre = 2)
				begin
				set @inicio = '01-04-'+@AnioAux
				set @fin = '30-06-'+@AnioAux
				end
				else if (@trimestre = 3)
					begin 
					set @inicio = '01-07-'+@AnioAux
					set @fin = '30-09-'+@AnioAux
					end
					else if (@trimestre = 4)
						begin
						set @inicio = '01-10-'+@AnioAux
						set @fin = '31-12-'+@AnioAux
						end
						else 
							begin
							set @inicio = '01-01-'+@AnioAux
							set @fin = '31-12-'+@AnioAux
							end

select top 5 consTotal.hot, hot.nombre ,SUM(consTotal.Dias) Dias_Baja from 

	( select * from

			(select ho.idHotel hot, SUM(DATEDIFF(day,bh.fecha_ini,bh.fecha_hasta)) Dias
			from GESTION_DE_GATOS.BajaHotel bh,
					GESTION_DE_GATOS.Hotel ho
			where bh.hotel = ho.idHotel 
					and bh.fecha_ini between @inicio and @fin
					and bh.fecha_hasta < @fin
			group by ho.idHotel ) as consPre
		
		UNION ALL

	select * from

			(select ho.idHotel hot, SUM(DATEDIFF(day,bh.fecha_ini,@fin)) Dias
			from GESTION_DE_GATOS.BajaHotel bh,
					GESTION_DE_GATOS.Hotel ho
			where bh.hotel = ho.idHotel 
					and bh.fecha_ini between @inicio and @fin
					and bh.fecha_hasta > @fin
			group by ho.idHotel) as consPost
		) as consTotal,
		GESTION_DE_GATOS.Hotel hot
		where hot.idHotel = consTotal.hot
		group by consTotal.hot, hot.nombre
		order by Dias_Baja desc
	end
	go
--------------------------------------------------------------
Create procedure GESTION_DE_GATOS.lista_hoteles_maxConFacturados
				@trimestre numeric(18,0), @Anio numeric(18,0) as
				
				begin
		
		declare @inicio datetime
		declare @fin datetime
		declare @AnioAux char(4)
		set @AnioAux = CAST(@Anio as CHAR(4))
		
		if (@trimestre = 1)
			begin
			set @inicio = '01-01-'+@AnioAux
			set @fin = '31-03-'+@AnioAux
			end
			else if (@trimestre = 2)
				begin
				set @inicio = '01-04-'+@AnioAux
				set @fin = '30-06-'+@AnioAux
				end
				else if (@trimestre = 3)
					begin 
					set @inicio = '01-07-'+@AnioAux
					set @fin = '30-09-'+@AnioAux
					end
					else if (@trimestre = 4)
						begin
						set @inicio = '01-10-'+@AnioAux
						set @fin = '31-12-'+@AnioAux
						end
						else 
							begin
							set @inicio = '01-01-'+@AnioAux
							set @fin = '31-12-'+@AnioAux
							end

				select top 5
					ho.nombre Nombre_Hotel,
					count(con.idConsumible) Cons_Facturados
				from GESTION_DE_GATOS.Consumibles con,
					GESTION_DE_GATOS.ConsumibleXEstadia cxe,
					GESTION_DE_GATOS.Estadia es,
					GESTION_DE_GATOS.Factura fc,
					GESTION_DE_GATOS.Hotel ho,
					GESTION_DE_GATOS.Reserva res,
					GESTION_DE_GATOS.ReservaXHabitacion rxh,
					GESTION_DE_GATOS.Habitacion hab
				where 
					fc.estadia = es.idEstadia and
					cxe.estadia = es.idEstadia and
					cxe.consumible = con.idConsumible and
					es.reserva = res.idReserva and
					res.idReserva = rxh.reserva and
					rxh.habitacion = hab.idHabitacion and
					hab.hotel = ho.idHotel
					and fc.fecha between @inicio AND @fin
				group by ho.nombre
				order by Cons_Facturados desc
				end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarUserXRolXHotel

@user numeric(18,0),
@hotel numeric(18,0),
@rolR nvarchar(255)

as
begin
declare @rol numeric(18,0)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	set @rol = (select idRol from GESTION_DE_GATOS.Rol where descripcion = @rolR) 
	insert into GESTION_DE_GATOS.UserXRolXHotel(usuario,rol,hotel) 
	values(@user,@rol,@hotel);
	
	set @respuesta = (select SCOPE_IDENTITY());
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarHotel
@nombre nvarchar(45),
@mail nvarchar(60),
@telefono numeric(18,0),
@direccion numeric(18,0),
@cant_estrellas numeric(18,0),
@recarga_estrella numeric(18,0),
@fechaI datetime
as
begin


declare @fecha_creacion datetime
Set @fecha_creacion = CONVERT(datetime,@fechaI,121)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	insert into GESTION_DE_GATOS.Hotel (nombre,mail,telefono,direccion,cant_estrellas,recarga_estrella,fecha_creacion) 
	values(@nombre,@mail,@telefono,@direccion,@cant_estrellas,@recarga_estrella,@fecha_creacion);
	
	set @respuesta = (select SCOPE_IDENTITY());
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.InsertarRegimenXHotel
@regimen numeric(18,0),
@hotel numeric(18,0)


as
begin

declare @respuesta numeric(18,0)
begin tran ta
begin try
	
	insert into GESTION_DE_GATOS.RegimenXHotel(regimen,hotel) 
	values(@regimen,@hotel);
	
	set @respuesta = (select SCOPE_IDENTITY());
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.ModificarHotel
@id numeric(18,0),
@nombre nvarchar(45),
@mail nvarchar(60),
@telefono numeric(18,0),
@direccion numeric(18,0),
@cant_estrellas numeric(18,0),
@recarga_estrella numeric(18,0),
@fechaI datetime
as
begin

declare @fecha_creacion datetime
Set @fecha_creacion = CONVERT(datetime,@fechaI,121)
declare @respuesta numeric(18,0)
begin tran ta
begin try
	update GESTION_DE_GATOS.Hotel
	set nombre =@nombre,
	mail=@mail,
	telefono=@telefono,
	direccion=@direccion,
	cant_estrellas = @cant_estrellas,
	recarga_estrella = @recarga_estrella,
	fecha_creacion=@fecha_creacion
	where
	idHotel = @id
	set @respuesta = @id;
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------
create proc GESTION_DE_GATOS.BorrarRegimenesXHotel
@id numeric(18,0)
as
begin

declare @respuesta numeric(18,0)
begin tran ta
begin try
	delete from GESTION_DE_GATOS.RegimenXHotel
	where
	hotel = @id
	set @respuesta = 1
	select @respuesta as respuesta
commit tran ta
end try
begin catch
rollback tran ta
set @respuesta=0
select @respuesta as respuesta
end catch
end
GO
--------------------------------------------------------------

----------------------------------------------------------------------------------------------------------
								/*CREACION DE LAS TABLAS*/

create table GESTION_DE_GATOS.Pais(
	idPais numeric(18,0) identity(1,1) NOT NULL,
	nombre nvarchar(30) NOT NULL,
	)
	
create table GESTION_DE_GATOS.Estado(
	idEstado numeric(18,0) identity(1,1) NOT NULL,
	descripcion nvarchar(40) NOT NULL,
	)
	
create table GESTION_DE_GATOS.Direccion(
	idDir numeric(18,0) IDENTITY(1,1) NOT NULL,
	calle nvarchar(255) NOT NULL,
	numero numeric(18,0) NOT NULL,
	piso numeric(18,0),
	depto nvarchar(50),
	ciudad nvarchar(255),
	pais numeric(18,0) NOT NULL,
	)
	
create table GESTION_DE_GATOS.TiposDoc(
	idTipoDoc numeric(18,0) IDENTITY(1,1) NOT NULL,
	descripcion nvarchar(40) unique NOT NULL,
	)
	
		
create table GESTION_DE_GATOS.Funcionalidad(
	idFuncionalidad numeric(18,0) identity(1,1) NOT NULL,
	denominacion nvarchar(45) NOT NULL,
	)
		
create table GESTION_DE_GATOS.Regimen(
	codigo numeric(18,0) identity(1,1) NOT NULL,
	descripcion nvarchar(255) NOT NULL,
	precio numeric(18,2) NOT NULL,
	estado bit NOT NULL,
	)
		
create table GESTION_DE_GATOS.Rol(
	idRol numeric(18,0) identity(1,1) NOT NULL,
	descripcion nvarchar(45) unique NOT NULL,
	estado bit NOT NULL,
	)
	
create table GESTION_DE_GATOS.FormaDePago(
	idFormaDePago numeric(18,0) IDENTITY(1,1) NOT NULL,
	descripcion nvarchar(50) NOT NULL,
	)
	
create table GESTION_DE_GATOS.Consumibles(
	idConsumible numeric(18,0) identity(2324,1) NOT NULL,
	descripcion nvarchar(255) NOT NULL,
	precio numeric(18,2) NOT NULL,
	)
		
create table GESTION_DE_GATOS.TipoHabitacion(
	codigo numeric(18,0) identity(1001,1) NOT NULL,
	descripcion nvarchar(255) unique NOT NULL,
	porcentual numeric(18,2) NOT NULL,
	cantPersonas numeric(18,0),
	)

create table GESTION_DE_GATOS.Cliente(
	idCli numeric(18,0) IDENTITY(1,1) NOT NULL,
	tipoDoc numeric(18,0) NOT NULL,
	nroDoc numeric(18,0) NOT NULL,
	apellido nvarchar(255) NOT NULL,
	nombre nvarchar(255) NOT NULL,
	fecha_nac DATETIME NOT NULL,
	mail nvarchar(255) NOT NULL,
	direccion numeric(18,0) NOT NULL,
	nacionalidad numeric(18,0) NOT NULL,
	habilitado bit NOT NULL,
	telefono numeric(18,0) NOT NULL,
	)
	
create table GESTION_DE_GATOS.ClienteXEstadia(
	idClienteXEstadia numeric(18,0) IDENTITY(1,1) NOT NULL,
	cliente numeric(18,0),
	clienteError numeric(18,0),
	estadia numeric(18,0) NOT NULL,
	habitacion numeric(18,0)
	)
	
create table GESTION_DE_GATOS.Factura(
	numero numeric(18,0) IDENTITY(2396745,1) NOT NULL,
	estadia numeric(18,0) NOT NULL,
	formaDePago numeric(18,0) NOT NULL,
	cliente numeric(18,0),
	clienteError numeric(18,0),
	total numeric(18,2) NOT NULL,
	tarjeta numeric(18,0),
	fecha DateTime NOT NULL,
	)
create table GESTION_DE_GATOS.ItemFactura(
	id numeric(18,0) IDENTITY(1,1) NOT NULL,
	factura numeric(18,0) NOT NULL,
	cantidad numeric(18,0) NOT NULL,
	monto numeric(18,2) NOT NULL,
	descripcion nvarchar(255) NOT NULL,
	)

create table GESTION_DE_GATOS.Tarjeta(
	idTarjeta numeric(18,0) identity(1,1) not null,
	entidad nvarchar(100) ,
	numero numeric(18,0) unique not null,
	banco nvarchar(100),
	titular nvarchar(100),
	)
	
create table GESTION_DE_GATOS.Estadia(
	idEstadia numeric(18,0) IDENTITY(1,1) NOT NULL,
	reserva numeric(18,0) NOT NULL,
	salida datetime,
	ingreso datetime,
	dias_sobran numeric(18,0),
	usuario numeric(18,0) NOT NULL,
	usuarioSalida numeric(18,0),
	cantidadNoches numeric(18,0) NOT NULL,
	precioPorNoche numeric(18,0) NOT NULL,
	)
	
create table GESTION_DE_GATOS.Usuario(
	idUsuario numeric(18,0) identity(1,1) NOT NULL,
	nombre nvarchar(50) NOT NULL,
	clave nvarchar(255) NOT NULL,
	apellido nvarchar(50) NOT NULL,
	fecha_nac datetime NOT NULL,
	mail nvarchar(50) NOT NULL,
	direccion numeric(18,0) NOT NULL,
	estado bit NOT NULL,
	telefono numeric(18,0) NOT NULL,
	tipoDoc numeric(18,0) NOT NULL,
	nroDoc numeric(18,0) NOT NULL,
	userName nvarchar(30) unique NOT NULL,
	)

create table GESTION_DE_GATOS.Reserva(
	idReserva numeric(18,0) identity(10001,1) NOT NULL,
	fecha_inicio datetime NOT NULL,
	regimen numeric(18,0) NOT NULL,
	fecha_hasta datetime,
	fecha_registro datetime NOT NULL,
	estado numeric(18,0),
	usuario numeric(18,0) NOT NULL,
	cliente numeric(18,0),
	clienteError numeric(18,0),
	costoTotal numeric(18,0),
	cantNoches numeric(18,0) NOT NULL,
	cantPersonas numeric(18,0),
	)

create table GESTION_DE_GATOS.Modificacion(
	idModif numeric(18,0) identity(1,1) NOT NULL,
	usuario numeric(18,0) NOT NULL,
	reserva numeric(18,0) NOT NULL,
	dia datetime,
	);
	
create table GESTION_DE_GATOS.Cancelacion(
	idCancel numeric(18,0) identity(1,1) NOT NULL,
	usuario numeric(18,0) NOT NULL,
	reserva numeric(18,0) NOT NULL,
	dia datetime,
	motivo nvarchar(60),
	);
	
create table GESTION_DE_GATOS.Habitacion(
	idHabitacion numeric(18,0) identity(1,1) NOT NULL,
	numero numeric(18,0) NOT NULL,
	piso numeric(18,0) NOT NULL,
	ubicacion nvarchar(20),
	tipo numeric(18,0) NOT NULL,
	hotel numeric(18,0) NOT NULL,
	descripcion nvarchar(100),
	estado bit NOT NULL,
	)
	
create table GESTION_DE_GATOS.ReservaXHabitacion(
	idReservaXHabitacion numeric(18,0) identity(1,1) NOT NULL,
	habitacion numeric(18,0) NOT NULL,
	reserva numeric(18,0) NOT NULL,
	)
	
create table GESTION_DE_GATOS.BajaHotel(
	idBajaHotel numeric(18,0) identity(1,1) NOT NULL,
	hotel numeric(18,0) NOT NULL,
	fecha_ini datetime NOT NULL,
	fecha_hasta datetime NOT NULL,
	motivo nvarchar(60) NOT NULL,
	)
	
create table GESTION_DE_GATOS.Hotel(
	idHotel numeric(18,0) identity(1,1) NOT NULL,
	nombre nvarchar(45) unique NOT NULL,
	mail nvarchar(60),
	telefono numeric(18,0),
	direccion numeric(18,0) NOT NULL,
	cant_estrellas numeric(18,0) NOT NULL,
	recarga_estrella numeric(18,0) NOT NULL,
	fecha_creacion datetime,
	)

	
create table GESTION_DE_GATOS.ConsumibleXEstadia(
	idConsumibleXEstadia numeric(18,0) identity(1,1) NOT NULL,
	estadia numeric(18,0) NOT NULL,
	consumible numeric(18,0) NOT NULL,
	habitacion numeric(18,0) NOT NULL,
	item numeric(18,0) NOT NULL,
	)

	
create table GESTION_DE_GATOS.RegimenXHotel(
	idRegimenXHotel numeric(18,0) identity(1,1) NOT NULL,
	regimen numeric(18,0) NOT NULL,
	hotel numeric(18,0) NOT NULL,
	)

	
create table GESTION_DE_GATOS.UserXRolXHotel(
	idUserXRol numeric(18,0) identity(1,1) NOT NULL,
	usuario numeric(18,0) NOT NULL,
	rol numeric(18,0) NOT NULL,
	hotel numeric(18,0) NOT NULL,
	)

	
create table GESTION_DE_GATOS.FuncXRol(
	idFuncXRol numeric(18,0) identity(1,1) NOT NULL,
	rol numeric(18,0) NOT NULL,
	funcionalidad numeric(18,0) NOT NULL,
	)
	
	-- Creo tabla para almacenar clientes con mails duplicados

create table GESTION_DE_GATOS.ClientesError(
	idCli numeric(18,0) IDENTITY(1,1) NOT NULL,
	tipoDoc numeric(18,0) NOT NULL,
	nroDoc numeric(18,0) NOT NULL,
	apellido nvarchar(255) NOT NULL,
	nombre nvarchar(255) NOT NULL,
	fecha_nac DATETIME,
	mail nvarchar(255) NOT NULL,
	direccion numeric(18,0) NOT NULL,
	nacionalidad numeric(18,0) NOT NULL,
	habilitado bit NOT NULL,
	telefono numeric(18,0) NOT NULL,
	conflicto nvarchar(255) NOT NULL,
	)
---------------------------------------------------------------------------------------------------------------------------------------------------
							/*CREANDO CONSTRAINT PK PARA LAS TABLAS DE LA BASE DE DATOS*/
							
/*AGREGANDO CONSTRAINT PK A LA TABLA Pais*/
ALTER TABLE GESTION_DE_GATOS.Pais
ADD CONSTRAINT PK_Pais_idPais PRIMARY KEY (idPais)

/*AGREGANDO CONSTRAINT PK A LA TABLA Tarjeta*/
ALTER TABLE GESTION_DE_GATOS.Tarjeta
ADD CONSTRAINT PK_tarjeta_idtarjeta PRIMARY KEY (idTarjeta)

/*AGREGANDO CONSTRAINT PK A LA TABLA Estado*/
ALTER TABLE GESTION_DE_GATOS.Estado
ADD CONSTRAINT PK_Estado_idEstado PRIMARY KEY (idEstado)

/*AGREGANDO CONSTRAINT PK A LA TABLA DIRECCION*/
ALTER TABLE GESTION_DE_GATOS.Direccion
ADD CONSTRAINT PK_Direccion_idDir PRIMARY KEY (idDir)

/*AGREGANDO CONSTRAINT PK A LA TABLA TiposDoc*/
ALTER TABLE GESTION_DE_GATOS.TiposDoc
ADD CONSTRAINT PK_TiposDoc_idTipoDoc PRIMARY KEY (idTipoDoc)

/*AGREGANDO CONSTRAINT PK A LA TABLA Funcionalidad*/
ALTER TABLE GESTION_DE_GATOS.Funcionalidad
ADD CONSTRAINT PK_Funcionalidad_idFuncionalidad PRIMARY KEY (idFuncionalidad)

/*AGREGANDO CONSTRAINT PK A LA TABLA Regimen*/
ALTER TABLE GESTION_DE_GATOS.Regimen
ADD CONSTRAINT PK_Regimen_codigo PRIMARY KEY (codigo)

/*AGREGANDO CONSTRAINT PK A LA TABLA Rol*/
ALTER TABLE GESTION_DE_GATOS.Rol
ADD CONSTRAINT PK_Rol_idRol PRIMARY KEY (idRol)

/*AGREGANDO CONSTRAINT PK A LA TABLA FormaDePago*/
ALTER TABLE GESTION_DE_GATOS.FormaDePago
ADD CONSTRAINT PK_FormaDePago_idFormaDePago PRIMARY KEY (idFormaDePago)

/*AGREGANDO CONSTRAINT PK A LA TABLA Consumibles*/
ALTER TABLE GESTION_DE_GATOS.Consumibles
ADD CONSTRAINT PK_Consumibles_idConsumible PRIMARY KEY (idConsumible)

/*AGREGANDO CONSTRAINT PK A LA TABLA TipoHabitacion*/
ALTER TABLE GESTION_DE_GATOS.TipoHabitacion
ADD CONSTRAINT PK_TipoHabitacion_codigo PRIMARY KEY (codigo)

/*AGREGANDO CONSTRAINT PK A LA TABLA Cliente*/
ALTER TABLE GESTION_DE_GATOS.Cliente
ADD CONSTRAINT PK_Cliente_idCli PRIMARY KEY (idCli)

/*AGREGANDO CONSTRAINT PK A LA TABLA ClienteXEstadia*/
ALTER TABLE GESTION_DE_GATOS.ClienteXEstadia
ADD CONSTRAINT PK_ClienteXEstadia_idClienteXEstadia PRIMARY KEY (idClienteXEstadia)

/*AGREGANDO CONSTRAINT PK A LA TABLA Factura*/
ALTER TABLE GESTION_DE_GATOS.Factura
ADD CONSTRAINT PK_Factura_numero PRIMARY KEY (numero)

/*AGREGANDO CONSTRAINT PK A LA TABLA ItemFactura*/
ALTER TABLE GESTION_DE_GATOS.ItemFactura
ADD CONSTRAINT PK_ItemFactura_id PRIMARY KEY (id)

/*AGREGANDO CONSTRAINT PK A LA TABLA Estadia*/
ALTER TABLE GESTION_DE_GATOS.Estadia
ADD CONSTRAINT PK_Estadia_idEstadia PRIMARY KEY (idEstadia)

/*AGREGANDO CONSTRAINT PK A LA TABLA Usuario*/
ALTER TABLE GESTION_DE_GATOS.Usuario
ADD CONSTRAINT PK_Usuario_idUsuario PRIMARY KEY (idUsuario)

/*AGREGANDO CONSTRAINT PK A LA TABLA Reserva*/
ALTER TABLE GESTION_DE_GATOS.Reserva
ADD CONSTRAINT PK_Reserva_idReserva PRIMARY KEY (idReserva)

/*AGREGANDO CONSTRAINT PK A LA TABLA Modificacion*/
ALTER TABLE GESTION_DE_GATOS.Modificacion
ADD CONSTRAINT PK_Modificacion_idModif PRIMARY KEY (idModif)

/*AGREGANDO CONSTRAINT PK A LA TABLA Cancelacion*/
ALTER TABLE GESTION_DE_GATOS.Cancelacion
ADD CONSTRAINT PK_Cancelacion_idCancel PRIMARY KEY (idCancel)

/*AGREGANDO CONSTRAINT PK A LA TABLA Habitacion*/
ALTER TABLE GESTION_DE_GATOS.Habitacion
ADD CONSTRAINT PK_Habitacion_idHabitacion PRIMARY KEY (idHabitacion)

/*AGREGANDO CONSTRAINT PK A LA TABLA ReservaXHabitacion*/
ALTER TABLE GESTION_DE_GATOS.ReservaXHabitacion
ADD CONSTRAINT PK_ReservaXHabitacion_idReservaXHabitacion PRIMARY KEY (idReservaXHabitacion)

/*AGREGANDO CONSTRAINT PK A LA TABLA BajaHotel*/
ALTER TABLE GESTION_DE_GATOS.BajaHotel
ADD CONSTRAINT PK_BajaHotel_idBajaHotel PRIMARY KEY (idBajaHotel)

/*AGREGANDO CONSTRAINT PK A LA TABLA Hotel*/
ALTER TABLE GESTION_DE_GATOS.Hotel
ADD CONSTRAINT PK_Hotel_idHotel PRIMARY KEY (idHotel)

/*AGREGANDO CONSTRAINT PK A LA TABLA ConsumibleXEstadia*/
ALTER TABLE GESTION_DE_GATOS.ConsumibleXEstadia
ADD CONSTRAINT PK_ConsumibleXEstadia_idConsumibleXEstadia PRIMARY KEY (idConsumibleXEstadia)

/*AGREGANDO CONSTRAINT PK A LA TABLA RegimenXHotel*/
ALTER TABLE GESTION_DE_GATOS.RegimenXHotel
ADD CONSTRAINT PK_RegimenXHotel_idRegimenXHotel PRIMARY KEY (idRegimenXHotel)

/*AGREGANDO CONSTRAINT PK A LA TABLA UserXRol*/
ALTER TABLE GESTION_DE_GATOS.UserXRolXHotel
ADD CONSTRAINT PK_UserXRolXHotel_idUserXRol PRIMARY KEY (idUserXRol)

/*AGREGANDO CONSTRAINT PK A LA TABLA FuncXRol*/
ALTER TABLE GESTION_DE_GATOS.FuncXRol
ADD CONSTRAINT PK_FuncXRol_idFuncXRol PRIMARY KEY (idFuncXRol)

/*AGREGANDO CONSTRAINT PK A LA TABLA ClientesError*/
ALTER TABLE GESTION_DE_GATOS.ClientesError
ADD CONSTRAINT PK_ClientesError_idCli PRIMARY KEY (idCli)

-----------------------------------------------------------------------------------------------------------------------------------------------------

							/*CREANDO CONSTRAINT FK PARA LAS TABLAS DE LA BASE DE DATOS */

-----------------------------------------------------------------------------------------------------------------------------------------------------

/*AGREGANDO CONSTRAINS FK A LA TABLA Direccion-Pais*/
ALTER TABLE GESTION_DE_GATOS.Direccion
ADD CONSTRAINT FK_Direccion_Pais FOREIGN KEY(pais) REFERENCES GESTION_DE_GATOS.Pais(idPais) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Cliente-Pais*/
ALTER TABLE GESTION_DE_GATOS.Cliente
ADD CONSTRAINT FK_Cliente_Pais FOREIGN KEY(nacionalidad) REFERENCES GESTION_DE_GATOS.Pais(idPais) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Cliente-TiposDoc*/
ALTER TABLE GESTION_DE_GATOS.Cliente
ADD CONSTRAINT FK_Cliente_TiposDoc FOREIGN KEY(tipoDoc) REFERENCES GESTION_DE_GATOS.TiposDoc(idTipoDoc) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Cliente-DIRECCION*/
ALTER TABLE GESTION_DE_GATOS.Cliente
ADD CONSTRAINT FK_Cliente_Direccion FOREIGN KEY(direccion) REFERENCES GESTION_DE_GATOS.Direccion(idDir)

/*AGREGANDO CONSTRAINS FK A LA TABLA Usuario-TiposDoc*/
ALTER TABLE GESTION_DE_GATOS.Usuario
ADD CONSTRAINT FK_Usuario_TiposDoc FOREIGN KEY(tipoDoc) REFERENCES GESTION_DE_GATOS.TiposDoc(idTipoDoc) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Usuario-DIRECCION*/
ALTER TABLE GESTION_DE_GATOS.Usuario
ADD CONSTRAINT FK_Usuario_Direccion FOREIGN KEY(direccion) REFERENCES GESTION_DE_GATOS.Direccion(idDir) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Estadia-Usuario*/
ALTER TABLE GESTION_DE_GATOS.Estadia
ADD CONSTRAINT FK_Estadia_Usuario FOREIGN KEY(usuario) REFERENCES GESTION_DE_GATOS.Usuario(idUsuario) 

/*AGREGANDO CONSTRAINS FK A LA TABLA Estadia-Usuario*/
ALTER TABLE GESTION_DE_GATOS.Estadia
ADD CONSTRAINT FK_Estadia_UsuarioSalida FOREIGN KEY(usuarioSalida) REFERENCES GESTION_DE_GATOS.Usuario(idUsuario) 

/*AGREGANDO CONSTRAINS FK A LA TABLA Estadia-Reserva*/
ALTER TABLE GESTION_DE_GATOS.Estadia
ADD CONSTRAINT FK_Estadia_Reserva FOREIGN KEY(reserva) REFERENCES GESTION_DE_GATOS.Reserva(idReserva) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ClienteXEstadia-Cliente*/
ALTER TABLE GESTION_DE_GATOS.ClienteXEstadia
ADD CONSTRAINT FK_ClienteXEstadia_Cliente FOREIGN KEY(cliente) REFERENCES GESTION_DE_GATOS.Cliente(idCli) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ClienteXEstadia-ClientesError*/
ALTER TABLE GESTION_DE_GATOS.ClienteXEstadia
ADD CONSTRAINT FK_ClienteXEstadia_ClienteError FOREIGN KEY(clienteError) REFERENCES GESTION_DE_GATOS.ClientesError(idCli)

alter table GESTION_DE_GATOS.ClienteXEstadia
add constraint unique_registro_cliXest unique (cliente,estadia)

/*AGREGANDO CONSTRAINS FK A LA TABLA ClienteXEstadia-Habitacion*/
ALTER TABLE GESTION_DE_GATOS.ClienteXEstadia
ADD CONSTRAINT FK_ClienteXEstadia_Habitacion FOREIGN KEY(habitacion) REFERENCES GESTION_DE_GATOS.Habitacion(idHabitacion)

/*AGREGANDO CONSTRAINS FK A LA TABLA ClienteXEstadia-Estadia*/
ALTER TABLE GESTION_DE_GATOS.ClienteXEstadia
ADD CONSTRAINT FK_ClienteXEstadia_Estadia FOREIGN KEY(estadia) REFERENCES GESTION_DE_GATOS.Estadia(idEstadia)

/*AGREGANDO CONSTRAINS FK A LA TABLA Factura-FormaDePago*/
ALTER TABLE GESTION_DE_GATOS.Factura
ADD CONSTRAINT FK_Factura_FormaDePago FOREIGN KEY(formaDePago) REFERENCES GESTION_DE_GATOS.FormaDePago(idFormaDePago) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Factura-Estadia*/
ALTER TABLE GESTION_DE_GATOS.Factura
ADD CONSTRAINT FK_Factura_Estadia FOREIGN KEY(estadia) REFERENCES GESTION_DE_GATOS.Estadia(idEstadia) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Factura-Cliente*/
ALTER TABLE GESTION_DE_GATOS.Factura
ADD CONSTRAINT FK_Factura_Cliente FOREIGN KEY(cliente) REFERENCES GESTION_DE_GATOS.Cliente(idCli)

/*AGREGANDO CONSTRAINS FK A LA TABLA Factura-ClientesError*/
ALTER TABLE GESTION_DE_GATOS.Factura
ADD CONSTRAINT FK_Factura_ClientesError FOREIGN KEY(clienteError) REFERENCES GESTION_DE_GATOS.ClientesError(idCli)

/*AGREGANDO CONSTRAINS FK A LA TABLA Factura-Tarjeta*/
ALTER TABLE GESTION_DE_GATOS.Factura
ADD CONSTRAINT FK_Factura_Tarjeta FOREIGN KEY(tarjeta) REFERENCES GESTION_DE_GATOS.Tarjeta(idTarjeta)

/*AGREGANDO CONSTRAINS FK A LA TABLA ItemFactura-Factura*/
ALTER TABLE GESTION_DE_GATOS.ItemFactura
ADD CONSTRAINT FK_ItemFactura_Factura FOREIGN KEY(factura) REFERENCES GESTION_DE_GATOS.Factura(numero)

/*AGREGANDO CONSTRAINS FK A LA TABLA Reserva-Regimen*/
ALTER TABLE GESTION_DE_GATOS.Reserva
ADD CONSTRAINT FK_Reserva_Regimen FOREIGN KEY(regimen) REFERENCES GESTION_DE_GATOS.Regimen(codigo) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Reserva-Cliente*/
ALTER TABLE GESTION_DE_GATOS.Reserva
ADD CONSTRAINT FK_Reserva_Cliente FOREIGN KEY(cliente) REFERENCES GESTION_DE_GATOS.Cliente(idCli)

/*AGREGANDO CONSTRAINS FK A LA TABLA Reserva-ClientesError*/
ALTER TABLE GESTION_DE_GATOS.Reserva
ADD CONSTRAINT FK_Reserva_ClientesError FOREIGN KEY(clienteError) REFERENCES GESTION_DE_GATOS.ClientesError(idCli)

/*AGREGANDO CONSTRAINS FK A LA TABLA Reserva-Usuario*/
ALTER TABLE GESTION_DE_GATOS.Reserva
ADD CONSTRAINT FK_Reserva_Usuario FOREIGN KEY(usuario) REFERENCES GESTION_DE_GATOS.Usuario(idUsuario) 

/*AGREGANDO CONSTRAINS FK A LA TABLA Reserva-Estado*/
ALTER TABLE GESTION_DE_GATOS.Reserva
ADD CONSTRAINT FK_Reserva_Estado FOREIGN KEY(estado) REFERENCES GESTION_DE_GATOS.Estado(idEstado) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Modificacion-Usuario*/
ALTER TABLE GESTION_DE_GATOS.Modificacion
ADD CONSTRAINT FK_Modificacion_Usuario FOREIGN KEY(usuario) REFERENCES GESTION_DE_GATOS.Usuario(idUsuario) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Modificacion-Reserva*/
ALTER TABLE GESTION_DE_GATOS.Modificacion
ADD CONSTRAINT FK_Modificacion_Reserva FOREIGN KEY(reserva) REFERENCES GESTION_DE_GATOS.Reserva(idReserva) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Cancelacion-Usuario*/
ALTER TABLE GESTION_DE_GATOS.Cancelacion
ADD CONSTRAINT FK_Cancelacion_Usuario FOREIGN KEY(usuario) REFERENCES GESTION_DE_GATOS.Usuario(idUsuario) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Cancelacion-Reserva*/
ALTER TABLE GESTION_DE_GATOS.Cancelacion
ADD CONSTRAINT FK_Cancelacion_Reserva FOREIGN KEY(reserva) REFERENCES GESTION_DE_GATOS.Reserva(idReserva) ON DELETE CASCADE ON UPDATE CASCADE

alter table GESTION_DE_GATOS.Cancelacion
add constraint unique_cancelacion_registro unique (reserva)

/*AGREGANDO CONSTRAINS FK A LA TABLA Hotel-Direccion*/
ALTER TABLE GESTION_DE_GATOS.Hotel
ADD CONSTRAINT FK_Hotel_Direccion FOREIGN KEY(direccion) REFERENCES GESTION_DE_GATOS.Direccion(idDir) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Habitacion-Hotel*/
ALTER TABLE GESTION_DE_GATOS.Habitacion
ADD CONSTRAINT FK_Habitacion_Hotel FOREIGN KEY(hotel) REFERENCES GESTION_DE_GATOS.Hotel(idHotel) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA Habitacion-TipoHabitacion*/
ALTER TABLE GESTION_DE_GATOS.Habitacion
ADD CONSTRAINT FK_Habitacion_TipoHabitacion FOREIGN KEY(tipo) REFERENCES GESTION_DE_GATOS.TipoHabitacion(codigo) ON DELETE CASCADE ON UPDATE CASCADE

ALTER TABLE GESTION_DE_GATOS.Habitacion
ADD CONSTRAINT Unique_registro unique(numero,piso,tipo,hotel)

/*AGREGANDO CONSTRAINS FK A LA TABLA ReservaXHabitacion-Habitacion*/
ALTER TABLE GESTION_DE_GATOS.ReservaXHabitacion
ADD CONSTRAINT FK_ReservaXHabitacion_Habitacion FOREIGN KEY(habitacion) REFERENCES GESTION_DE_GATOS.Habitacion(idHabitacion) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ReservaXHabitacion-Reserva*/
ALTER TABLE GESTION_DE_GATOS.ReservaXHabitacion
ADD CONSTRAINT FK_ReservaXHabitacion_Reserva FOREIGN KEY(reserva) REFERENCES GESTION_DE_GATOS.Reserva(idReserva) ON DELETE CASCADE ON UPDATE CASCADE

Alter table GESTION_DE_GATOS.ReservaXHabitacion
add constraint unique_ReservaXhabitacion_registro unique(reserva,habitacion)

/*AGREGANDO CONSTRAINS FK A LA TABLA BajaHotel-Hotel*/
ALTER TABLE GESTION_DE_GATOS.BajaHotel
ADD CONSTRAINT FK_BajaHotel_Hotel FOREIGN KEY(hotel) REFERENCES GESTION_DE_GATOS.Hotel(idHotel) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ConsumibleXEstadia-Consumibles*/
ALTER TABLE GESTION_DE_GATOS.ConsumibleXEstadia
ADD CONSTRAINT FK_ConsumibleXEstadia_Consumibles FOREIGN KEY(consumible) REFERENCES GESTION_DE_GATOS.Consumibles(idConsumible) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ConsumibleXEstadia-Estadia*/
ALTER TABLE GESTION_DE_GATOS.ConsumibleXEstadia
ADD CONSTRAINT FK_ConsumibleXEstadia_Estadia FOREIGN KEY(estadia) REFERENCES GESTION_DE_GATOS.Estadia(idEstadia) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ConsumibleXEstadia-Habitacion*/
ALTER TABLE GESTION_DE_GATOS.ConsumibleXEstadia
ADD CONSTRAINT FK_ConsumibleXEstadia_Habitacion FOREIGN KEY(habitacion) REFERENCES GESTION_DE_GATOS.Habitacion(idHabitacion)

/*AGREGANDO CONSTRAINS FK A LA TABLA ConsumibleXEstadia-ItemFactura*/
ALTER TABLE GESTION_DE_GATOS.ConsumibleXEstadia
ADD CONSTRAINT FK_ConsumibleXEstadia_ItemFactura FOREIGN KEY(item) REFERENCES GESTION_DE_GATOS.ItemFactura(id)

/*AGREGANDO CONSTRAINS FK A LA TABLA RegimenXHotel-Regimen*/
ALTER TABLE GESTION_DE_GATOS.RegimenXHotel
ADD CONSTRAINT FK_RegimenXHotel_Regimen FOREIGN KEY(regimen) REFERENCES GESTION_DE_GATOS.Regimen(codigo) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA RegimenXHotel-Hotel*/
ALTER TABLE GESTION_DE_GATOS.RegimenXHotel
ADD CONSTRAINT FK_RegimenXHotel_Hotel FOREIGN KEY(hotel) REFERENCES GESTION_DE_GATOS.Hotel(idHotel) ON DELETE CASCADE ON UPDATE CASCADE

ALTER TABLE GESTION_DE_GATOS.RegimenXHotel
ADD CONSTRAINT Unique_registro_RegimenXHotel unique(regimen,hotel)

/*AGREGANDO CONSTRAINS FK A LA TABLA UserXRol-Usuario*/
ALTER TABLE GESTION_DE_GATOS.UserXRolXHotel
ADD CONSTRAINT FK_UserXRol_Usuario FOREIGN KEY(usuario) REFERENCES GESTION_DE_GATOS.Usuario(idUsuario) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA UserXRol-Rol*/
ALTER TABLE GESTION_DE_GATOS.UserXRolXHotel
ADD CONSTRAINT FK_UserXRol_Rol FOREIGN KEY(rol) REFERENCES GESTION_DE_GATOS.Rol(idRol) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA UserXRol-hOTEL*/
ALTER TABLE GESTION_DE_GATOS.UserXRolXHotel
ADD CONSTRAINT FK_UserXRol_Hotel FOREIGN KEY(hotel) REFERENCES GESTION_DE_GATOS.Hotel(idHotel)

ALTER TABLE GESTION_DE_GATOS.UserXRolXHotel
ADD CONSTRAINT Unique_registro_UserXRolXHotel unique(usuario,hotel,rol)

/*AGREGANDO CONSTRAINS FK A LA TABLA FuncXRol-Funcionalidad*/
ALTER TABLE GESTION_DE_GATOS.FuncXRol
ADD CONSTRAINT FK_FuncXRol_Funcionalidad FOREIGN KEY(funcionalidad) REFERENCES GESTION_DE_GATOS.Funcionalidad(idFuncionalidad) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA FuncXRol-Rol*/
ALTER TABLE GESTION_DE_GATOS.FuncXRol
ADD CONSTRAINT FK_FuncXRol_Rol FOREIGN KEY(rol) REFERENCES GESTION_DE_GATOS.Rol(idRol) ON DELETE CASCADE ON UPDATE CASCADE

ALTER TABLE GESTION_DE_GATOS.FuncXRol
ADD CONSTRAINT Unique_registro_FuncXRol unique(funcionalidad,rol)

/*AGREGANDO CONSTRAINS FK A LA TABLA ClientesError-Pais*/
ALTER TABLE GESTION_DE_GATOS.ClientesError
ADD CONSTRAINT FK_ClientesError_Pais FOREIGN KEY(nacionalidad) REFERENCES GESTION_DE_GATOS.Pais(idPais) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ClientesError-TiposDoc*/
ALTER TABLE GESTION_DE_GATOS.ClientesError
ADD CONSTRAINT FK_ClientesError_TiposDoc FOREIGN KEY(tipoDoc) REFERENCES GESTION_DE_GATOS.TiposDoc(idTipoDoc) ON DELETE CASCADE ON UPDATE CASCADE

/*AGREGANDO CONSTRAINS FK A LA TABLA ClientesError-DIRECCION*/
ALTER TABLE GESTION_DE_GATOS.ClientesError
ADD CONSTRAINT FK_ClientesError_Direccion FOREIGN KEY(direccion) REFERENCES GESTION_DE_GATOS.Direccion(idDir)

------------------------------------------------------------------------------------------------------------------------------

--CARGA INICIAL PAISES
-- Carga inicial de paises con sus nacionalidades

insert into GESTION_DE_GATOS.Pais (nombre)
	values ('ARGENTINA');
	
insert into GESTION_DE_GATOS.Pais (nombre)
	values ('BRASIL');
	
insert into GESTION_DE_GATOS.Pais (nombre)
	values ('ITALIA');

------------------------------------------------------------------------------------------------------------------------------

-- CARGA INICIAL TIPOS DOC

insert into GESTION_DE_GATOS.TiposDoc(
			descripcion)
			values('Libreta Civica');

insert into GESTION_DE_GATOS.TiposDoc(
			descripcion)
			values('Documento Nacional de Identidad');			
		
insert into GESTION_DE_GATOS.TiposDoc(
			descripcion)
			values('Cedula de Identidad');
			
insert into GESTION_DE_GATOS.TiposDoc(
			descripcion)
			values('Pasaporte');
			
insert into GESTION_DE_GATOS.TiposDoc(
			descripcion)
			values('Partida de Nacimiento');
			
------------------------------------------------------------------------------------------------------------------------------	
	
--CARGA ESTADOS DE RESERVA

insert into GESTION_DE_GATOS.Estado (descripcion)
	values ('RESERVA CORRECTA');
	
insert into GESTION_DE_GATOS.Estado (descripcion)
	values ('RESERVA MODIFICADA');

insert into GESTION_DE_GATOS.Estado (descripcion)
	values ('RESERVA CANCELADA POR RECEPCION');

insert into GESTION_DE_GATOS.Estado (descripcion)
	values ('RESERVA CANCELADA POR CLIENTE');

insert into GESTION_DE_GATOS.Estado (descripcion)
	values ('RESERVA CANCELADA POR NO-SHOW');

insert into GESTION_DE_GATOS.Estado (descripcion)
	values ('RESERVA EFECTIVIZADA');	

insert into GESTION_DE_GATOS.Estado (descripcion)
	values ('RESERVA CANCELADA DESDE TABLA MAESTRA');	
	
------------------------------------------------------------------------------------------------------------------------------

--CARGA FUNCIONALIDADES

insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('ABM ROL');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('ABM USUARIO');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('ABM CLIENTE');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('ABM HOTEL');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('ABM HABITACION');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('ABM REGIMEN');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('GENERAR/MODIFICAR RESERVA');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('BAJA RESERVA');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('REGISTRAR ESTADIA');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('REGISTRAR CONSUMIBLES');
	
insert into GESTION_DE_GATOS.Funcionalidad (denominacion)
	values ('LISTADO ESTADISTICO');
------------------------------------------------------------------------------------------------------------------------------

--CARGA ROLES

insert into GESTION_DE_GATOS.ROL (descripcion,estado)
	values ('ADMINISTRADOR GENERAL',1);

insert into GESTION_DE_GATOS.ROL (descripcion,estado)
	values ('ADMINISTRADOR',1);

insert into GESTION_DE_GATOS.ROL (descripcion,estado)
	values ('RECEPCIONISTA',1);

insert into GESTION_DE_GATOS.ROL (descripcion,estado)
	values ('GUEST',1);
	
------------------------------------------------------------------------------------------------------------------------------
--CARGA FUNC X ROL

insert into GESTION_DE_GATOS.FuncXRol (rol,funcionalidad)
	select distinct R.idRol, F.idFuncionalidad from GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.Funcionalidad F
	where R.descripcion = 'ADMINISTRADOR GENERAL';

insert into GESTION_DE_GATOS.FuncXRol (rol,funcionalidad)
	select distinct R.idRol, F.idFuncionalidad from GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.Funcionalidad F
	where R.descripcion = 'ADMINISTRADOR' and
			F.denominacion in ('ABM HOTEL','ABM HABITACION','ABM REGIMEN','ABM USUARIO');

insert into GESTION_DE_GATOS.FuncXRol (rol,funcionalidad)
	select distinct R.idRol, F.idFuncionalidad from GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.Funcionalidad F
	where R.descripcion = 'RECEPCIONISTA' and
			F.denominacion in ('ABM CLIENTE','GENERAR/MODIFICAR RESERVA','BAJA RESERVA','REGISTRAR ESTADIA','REGISTRAR CONSUMIBLES');
			
insert into GESTION_DE_GATOS.FuncXRol (rol,funcionalidad)
	select distinct R.idRol, F.idFuncionalidad from GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.Funcionalidad F
	where R.descripcion = 'GUEST' and
			F.denominacion in ('GENERAR/MODIFICAR RESERVA','BAJA RESERVA');
			
------------------------------------------------------------------------------------------------------------------------------

--MIGRACION DIRECCIONES
--CARGAR PAISES ANTES
-- Cargar direcciones en tabla correspondiente
	
insert into GESTION_DE_GATOS.Direccion
 ( calle, numero, piso, depto, ciudad, pais)
 
 (
	select distinct Hotel_Calle, Hotel_Nro_Calle, NULL, 
				NULL, Hotel_Ciudad, P.idPais
			from gd_esquema.Maestra, GESTION_DE_GATOS.Pais P
			where Hotel_Calle IS NOT NULL and
			P.nombre = 'ARGENTINA'
			
			UNION ALL
			
	 select distinct Cliente_Dom_Calle, Cliente_Nro_Calle, Cliente_Piso, 
				Cliente_Depto, NULL, P.idPais 
			from gd_esquema.Maestra, GESTION_DE_GATOS.Pais P
			where Cliente_Dom_Calle IS NOT NULL and
			P.nombre = 'ARGENTINA'
	)
go

------------------------------------------------------------------------------------------------------------------------------
--MIGRACION TIPOHABITACION

SET IDENTITY_INSERT GESTION_DE_GATOS.TipoHabitacion ON
insert into GESTION_DE_GATOS.TipoHabitacion (
						codigo,
						descripcion,
						porcentual)
select distinct 	Habitacion_Tipo_Codigo,
					Habitacion_Tipo_Descripcion,
					Habitacion_Tipo_Porcentual 
from gd_esquema.Maestra
order by Habitacion_Tipo_Codigo
SET IDENTITY_INSERT GESTION_DE_GATOS.TipoHabitacion OFF

update GESTION_DE_GATOS.TipoHabitacion
	set cantPersonas = case when descripcion = 'Base Simple' Then 1
							when descripcion = 'Base Doble' Then 2
							when descripcion = 'Base Triple' Then 3
							when descripcion = 'Base Cuadruple' Then 4
							else 5 end
go

------------------------------------------------------------------------------------------------------------------------------

--MIGRACION REGIMEN

insert into GESTION_DE_GATOS.Regimen(
					descripcion,
					precio,
					estado
					)
select distinct Regimen_Descripcion,Regimen_Precio,1 from gd_esquema.Maestra
go

------------------------------------------------------------------------------------------------------------------------------

--MIGRACION CLIENTES
--CORRER MIGRACION DE PAISES,DIRECCIONES Y TIPOS DOC

-- Migro los datos de clientes de la tabla maestra

insert into GESTION_DE_GATOS.Cliente(
					direccion,
					tipoDoc,
					nacionalidad,
					nroDoc,
					apellido,
					nombre,
					fecha_nac,
					mail,
					habilitado,
					telefono
					)

select distinct D.idDir, 
				T.idTipoDoc, 
				P.idPais, 
				M.Cliente_Pasaporte_Nro,
				M.Cliente_Apellido,
				M.Cliente_Nombre,
				M.Cliente_Fecha_Nac,
				M.Cliente_Mail,
				1,
				000

from 	GESTION_DE_GATOS.Direccion D, 
		GESTION_DE_GATOS.TiposDoc T, 
		GESTION_DE_GATOS.Pais P, 
		gd_esquema.Maestra M
		
where 	D.calle = M.Cliente_Dom_Calle and
		D.numero = M.Cliente_Nro_Calle and
		D.piso = M.Cliente_Piso and
		D.depto = M.Cliente_Depto and
		T.descripcion = 'Pasaporte' and
		P.nombre = 'ARGENTINA'
	
		
-- exporto clientes con mismo mail a la tabla de clientes con error

insert into GESTION_DE_GATOS.ClientesError 
 (tipoDoc,nroDoc,apellido,nombre,fecha_nac,mail,direccion,nacionalidad,
	habilitado,telefono,conflicto) 
	(
select tipoDoc,nroDoc,apellido,nombre,fecha_nac,mail,direccion,nacionalidad,
	habilitado,telefono,'mail duplicado'
from GESTION_DE_GATOS.Cliente
Where idCli >
(select MIN(idCli)
from GESTION_DE_GATOS.Cliente as auxCli
where Cliente.mail = auxCli.mail
	)	
)


-- borro los clientes exportados de la lista de clientes

delete from GESTION_DE_GATOS.Cliente
Where idCli >
(
select MIN(idCli)
from GESTION_DE_GATOS.Cliente as auxCli
where Cliente.mail = auxCli.mail
)	

-- Agrego ahora la condicion de que mails sea unico en clientes

Alter table GESTION_DE_GATOS.Cliente
add constraint Dist_mail UNIQUE (mail);

-- Paso a clientes error los clientes que tengan mismo nro doc, dejo solo uno

insert into GESTION_DE_GATOS.ClientesError 
 (tipoDoc,nroDoc,apellido,nombre,fecha_nac,mail,direccion,nacionalidad,
	habilitado,telefono,conflicto) 
	(
select tipoDoc,nroDoc,apellido,nombre,fecha_nac,mail,direccion,nacionalidad,
	habilitado,telefono,'Nro Doc duplicado'
from GESTION_DE_GATOS.Cliente
Where idCli >
(
select MIN(idCli)
from GESTION_DE_GATOS.Cliente as auxCli
where Cliente.nroDoc = auxCli.nroDoc and Cliente.tipoDoc = auxCli.tipoDoc
)
)	


-- borro los clientes exportados de la lista de clientes

delete from GESTION_DE_GATOS.Cliente
Where idCli >
(
select MIN(idCli)
from GESTION_DE_GATOS.Cliente as auxCli
where Cliente.nroDoc = auxCli.nroDoc and Cliente.tipoDoc = auxCli.tipoDoc
)

go

------------------------------------------------------------------------------------------------------------------------------
--MIGRACION HOTEL
--CORRER ANTES LA MIGRACION DE DIRECCIONES
	
insert into GESTION_DE_GATOS.Hotel(
					direccion,
					nombre,
					cant_estrellas,
					recarga_estrella)
				
select distinct 	D.idDir, 
					GESTION_DE_GATOS.concatenarNombreHotel(M.Hotel_Calle,M.Hotel_Nro_Calle), 
					M.Hotel_CantEstrella, 
					M.Hotel_Recarga_Estrella

from GESTION_DE_GATOS.Direccion D, gd_esquema.Maestra M

where 	D.calle = M.Hotel_Calle and
		D.numero = M.Hotel_Nro_Calle and
		D.Ciudad = M.Hotel_Ciudad 

order by idDir
go		



------------------------------------------------------------------------------------------------------------------------------
--MIGRACION HABITACION
--CORRER ANTES MIGRACION DE HOTELES y TIPO HABITACION

insert into GESTION_DE_GATOS.Habitacion(
					numero,
					piso,
					ubicacion,
					tipo,
					hotel,
					estado
					)
select distinct M.Habitacion_Numero,
				M.Habitacion_Piso,
				M.Habitacion_Frente,
				M.Habitacion_Tipo_Codigo,
				H.idHotel,
				1
from 		gd_esquema.Maestra M,
			GESTION_DE_GATOS.Direccion D,
			GESTION_DE_GATOS.Hotel H
where 	D.calle = M.Hotel_Calle and
		D.numero = M.Hotel_Nro_Calle and
		D.ciudad = M.Hotel_Ciudad and
		D.idDir = H.direccion
order by idHotel,Habitacion_Piso,Habitacion_Numero

update GESTION_DE_GATOS.Habitacion
set ubicacion = case when ubicacion = 'S' Then 'Frente'
							else 'Sin Vista' end

go							

------------------------------------------------------------------------------------------------------------------------------

-- Creo usuario Guest

insert into GESTION_DE_GATOS.Usuario
	(nombre,clave,apellido,fecha_nac,mail,
	direccion,estado,telefono,tipoDoc,nroDoc,userName)
	values 
	('Guest',' ','Invitado','01-01-2014','n/a',1,1,000,1,00,'Guest')
	
insert into GESTION_DE_GATOS.Usuario
	(nombre,clave,apellido,fecha_nac,mail,
	direccion,estado,telefono,tipoDoc,nroDoc,userName)
	values 
	('ADMIN','e6b87050bfcb8143fcb8db0170a4dc9ed00d904ddd3e2a4ad1b1e8dc0fdc9be7','Administrador','01-01-2014','n/a',1,1,000,1,00,'admin')
	
insert into GESTION_DE_GATOS.UserXRolXHotel (usuario,rol,hotel)
	select distinct U.idUsuario, R.idRol,H.idHotel from GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.Usuario U,GESTION_DE_GATOS.Hotel H
	where R.descripcion = 'GUEST' and
		U.userName ='Guest';

insert into GESTION_DE_GATOS.UserXRolXHotel (usuario,rol,hotel)
	select distinct U.idUsuario, R.idRol, H.idHotel from GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.Usuario U,GESTION_DE_GATOS.Hotel H
	where R.descripcion = 'ADMINISTRADOR GENERAL' and
		U.userName ='admin';

	
	
--MIGRACION RESERVAS
--CORRER ANTES MIGRACION DE CLIENTES y REGIMEN

SET IDENTITY_INSERT GESTION_DE_GATOS.Reserva ON
insert into GESTION_DE_GATOS.Reserva(
					idReserva,
					fecha_inicio,
					regimen,
					cliente,
					cantNoches,
					fecha_registro,
					usuario
					)
select distinct 	M.Reserva_Codigo,
					M.Reserva_Fecha_Inicio,
					R.codigo,
					C.idCli,
					M.Reserva_Cant_Noches,
					getdate(),
					U.idUsuario
					
					
from GESTION_DE_GATOS.Cliente C, gd_esquema.Maestra M, GESTION_DE_GATOS.Regimen R,GESTION_DE_GATOS.Usuario U
where 	M.Cliente_Mail = C.mail and
		M.Cliente_Pasaporte_Nro = C.nroDoc and
		M.Cliente_Nombre = C.nombre and
		M.Cliente_Apellido = C.apellido and
		M.Regimen_Descripcion = R.descripcion and
		U.userName = 'Guest'
order by M.Reserva_Codigo

insert into GESTION_DE_GATOS.Reserva(
					idReserva,
					fecha_inicio,
					regimen,
					clienteError,
					cantNoches,
					fecha_registro,
					usuario
					)
select distinct 	M.Reserva_Codigo,
					M.Reserva_Fecha_Inicio,
					R.codigo,
					C1.idCli,
					M.Reserva_Cant_Noches,
					getdate(),
					U.idUsuario
					
from GESTION_DE_GATOS.ClientesError C1, gd_esquema.Maestra M, GESTION_DE_GATOS.Regimen R,GESTION_DE_GATOS.Usuario U
where 	M.Cliente_Mail = C1.mail and
		M.Cliente_Pasaporte_Nro = C1.nroDoc and
		M.Cliente_Nombre = C1.nombre and
		M.Cliente_Apellido = C1.apellido and
		M.Regimen_Descripcion = R.descripcion and
		U.userName = 'Guest'
order by M.Reserva_Codigo
SET IDENTITY_INSERT GESTION_DE_GATOS.Reserva OFF

--ACTUALIZACION FECHA HASTA DE RESERVA

update GESTION_DE_GATOS.Reserva
	set fecha_hasta = (fecha_inicio+cantNoches)

--ACTUALIZACION ESTADO DE RESERVA
	
update GESTION_DE_GATOS.Reserva
	set estado = CASE WHEN (idReserva= (select DISTINCT M.Reserva_Codigo from gd_esquema.Maestra M
						where 	idReserva = M.Reserva_Codigo and M.Factura_Nro IS not null and M.Estadia_Fecha_Inicio Is not null ) )
				THEN
					(select Es.idEstado from GESTION_DE_GATOS.Estado Es	
					where 	Es.Descripcion='RESERVA EFECTIVIZADA')
				ELSE 
					(select Es.idEstado from GESTION_DE_GATOS.Estado Es 
					where 	Es.Descripcion='RESERVA CANCELADA DESDE TABLA MAESTRA')
				end

GO
------------------------------------------------------------------------------------------------------------------------------
--MIGRACION REGIMENXHOTEL
	
insert into GESTION_DE_GATOS.RegimenXHotel(
								regimen,
								hotel)
select distinct R.codigo,
				H.idHotel
from gd_esquema.Maestra M, GESTION_DE_GATOS.Regimen R, GESTION_DE_GATOS.Hotel H, GESTION_DE_GATOS.Direccion D
where
	M.Hotel_Calle = D.calle AND
	M.Hotel_CantEstrella = H.cant_estrellas AND
	M.Hotel_Ciudad = D.ciudad AND
	M.Hotel_Nro_Calle = D.numero AND
	M.Hotel_Recarga_Estrella = H.recarga_estrella AND
	H.direccion = D.idDir and
	M.Regimen_Descripcion = R.descripcion AND
	M.Regimen_Precio = R.precio

GO

------------------------------------------------------------------------------------------------------------------------------
--MIGRACION RESERVA X HABITACION
--CORRER ANTES RESERVA Y HABITACION

insert into GESTION_DE_GATOS.ReservaXHabitacion(
								reserva,
								habitacion)
select distinct R.idReserva,
				Ha.idHabitacion
from gd_esquema.Maestra M, GESTION_DE_GATOS.Reserva R, GESTION_DE_GATOS.Habitacion Ha, GESTION_DE_GATOS.Direccion D, GESTION_DE_GATOS.Hotel H
where
-- asi encuentro el hotel
	M.Hotel_Calle = D.calle AND
	M.Hotel_CantEstrella = H.cant_estrellas AND
	M.Hotel_Ciudad = D.ciudad AND
	M.Hotel_Nro_Calle = D.numero AND
	M.Hotel_Recarga_Estrella = H.recarga_estrella AND
	H.direccion = D.idDir and
-- la habitacion
	Ha.numero = M.Habitacion_Numero and
	Ha.piso = M.Habitacion_Piso and
	Ha.Hotel = H.idHotel and
-- la reserva	
	M.Reserva_Codigo = R.idReserva

--ACTUALIZO LA CANTIDAD DE PERSONAS DE LA RESERVA
update GESTION_DE_GATOS.Reserva
	set cantPersonas = (select T.cantPersonas from GESTION_DE_GATOS.TipoHabitacion T,GESTION_DE_GATOS.Habitacion H,GESTION_DE_GATOS.ReservaXHabitacion R 
					where idReserva = R.reserva and
					R.habitacion = H.idHabitacion AND
					H.tipo = T.codigo)
	
--ACTUALIZO EL COSTO TOTAL DE LA RESERVA	

update GESTION_DE_GATOS.Reserva
	set costoTotal = ( (select precio from GESTION_DE_GATOS.Regimen R where regimen = R.codigo) * 
					(select T.porcentual from GESTION_DE_GATOS.TipoHabitacion T,GESTION_DE_GATOS.Habitacion H,GESTION_DE_GATOS.ReservaXHabitacion R 
					where idReserva = R.reserva and
					R.habitacion = H.idHabitacion AND
					H.tipo = T.codigo) +
					((select Ho.cant_estrellas from GESTION_DE_GATOS.Hotel Ho,GESTION_DE_GATOS.Habitacion H,GESTION_DE_GATOS.ReservaXHabitacion R 
					where idReserva = R.reserva and
					R.habitacion = H.idHabitacion AND
					Ho.idHotel = H.hotel) * 10) ) * cantNoches

go

------------------------------------------------------------------------------------------------------------------------------
-- MIGRACION DE LAS ESTADIAS

-- Deben estar migradas reservas, usuario

insert into GESTION_DE_GATOS.Estadia 
 (reserva, salida, ingreso, dias_sobran, usuario, cantidadNoches, precioPorNoche)
 
 ( 
 select distinct 
				m.Reserva_Codigo,
				m.Estadia_Fecha_Inicio + m.Estadia_Cant_Noches,
				m.Estadia_Fecha_Inicio,
				GESTION_DE_GATOS.calcEstadiaDiasSobran( res.fecha_hasta,m.Estadia_Fecha_Inicio + m.Estadia_Cant_Noches),
				us.idUsuario,
				m.Estadia_Cant_Noches,
				res.costoTotal / res.cantNoches
from GESTION_DE_GATOS.Reserva res,
	 gd_esquema.Maestra m,
	 GESTION_DE_GATOS.Usuario us
	 
where m.Reserva_Codigo = res.idReserva and
	us.idUsuario = res.usuario and
	m.Estadia_Fecha_Inicio is not null
)

order by m.Reserva_Codigo
GO

------------------------------------------------------------------------------------------------------------------------------
--MIGRACION CONSUMIBLES 

SET IDENTITY_INSERT GESTION_DE_GATOS.Consumibles ON
insert into GESTION_DE_GATOS.Consumibles (
						idConsumible,
						descripcion,
						precio)
select distinct Consumible_Codigo,Consumible_Descripcion,Consumible_Precio from gd_esquema.Maestra
where Consumible_Codigo is not null
order by Consumible_Codigo
SET IDENTITY_INSERT GESTION_DE_GATOS.Consumibles OFF
go
------------------------------------------------------------------------------------------------------------------------------

--CARGA FORMAS DE PAGO

insert into GESTION_DE_GATOS.FormaDePago (descripcion)
	values ('EFECTIVO');
	
insert into GESTION_DE_GATOS.FormaDePago (descripcion)
	values ('TARJETA DE CREDITO');

insert into GESTION_DE_GATOS.FormaDePago (descripcion)
	values ('TARJETA DE DEBITO');

------------------------------------------------------------------------------------------------------------------------------
--MIGRACION CLIENTESXESTADIA

insert into GESTION_DE_GATOS.ClienteXEstadia 
	(cliente,clienteError,estadia)
	
	(
	select distinct 
		cli.idCli,
		NULL,
		est.idEstadia
	from gd_esquema.Maestra m,
		 GESTION_DE_GATOS.Cliente cli,
		 GESTION_DE_GATOS.Estadia est
	where m.Estadia_Fecha_Inicio is not null and
			m.Cliente_Pasaporte_Nro = cli.nroDoc and
			m.Cliente_Apellido = cli.apellido and
			m.Cliente_Nombre = cli.nombre and
			m.Estadia_Fecha_Inicio = est.ingreso and
			m.Estadia_Cant_Noches = est.cantidadNoches and
			m.Reserva_Codigo = est.reserva
	
	UNION ALL
	
		select distinct 
			NULL,
			cliE.idCli,
			est.idEstadia
		from gd_esquema.Maestra m,
			 GESTION_DE_GATOS.ClientesError cliE,
			 GESTION_DE_GATOS.Estadia est
		where m.Estadia_Fecha_Inicio is not null and
			m.Cliente_Pasaporte_Nro = cliE.nroDoc and
			m.Cliente_Apellido = cliE.apellido and
			m.Cliente_Nombre = cliE.nombre and
			m.Estadia_Fecha_Inicio = est.ingreso and
			m.Estadia_Cant_Noches = est.cantidadNoches and
			m.Reserva_Codigo = est.reserva
	)
GO

update GESTION_DE_GATOS.ClienteXEstadia
set habitacion = 
(select distinct Ha.idHabitacion 
from GESTION_DE_GATOS.Estadia E, GESTION_DE_GATOS.Habitacion Ha,gd_esquema.Maestra M, GESTION_DE_GATOS.Hotel H
WHERE
estadia = E.idEstadia and
E.reserva = M.Reserva_Codigo and
H.nombre = M.Hotel_Calle+' '+CONVERT(nvarchar(255),M.Hotel_Nro_Calle)  and
Ha.hotel = H.idHotel and
Ha.numero = M.Habitacion_Numero and
Ha.piso = M.Habitacion_Piso)



------------------------------------------------------------------------------------------------------------------------------

--MIGRACION FACTURAS DE CLIENTES

SET IDENTITY_INSERT GESTION_DE_GATOS.Factura ON

insert into GESTION_DE_GATOS.Factura(
				numero,
				fecha,
				total,
				formaDePago,
				estadia,
				cliente)				

select distinct M.Factura_Nro,
				M.Factura_Fecha,
				M.Factura_Total+R.costoTotal,
				F.idFormaDePago,
				E.idEstadia,
				C.idCli

from	gd_esquema.Maestra M,
		GESTION_DE_GATOS.Reserva R,
		GESTION_DE_GATOS.Estadia E,
		GESTION_DE_GATOS.Cliente C,
		GESTION_DE_GATOS.FormaDePago F 

where	M.Factura_Nro is not null and
		M.Reserva_Codigo = R.idReserva and
		E.reserva = R.idReserva and
		C.idCli=R.cliente and
		F.descripcion = 'Efectivo'
		
		order by Factura_Nro
		
SET IDENTITY_INSERT GESTION_DE_GATOS.Factura OFF

--MIRACION FACTURAS DE CLIENTES CON ERROR

SET IDENTITY_INSERT GESTION_DE_GATOS.Factura ON

insert into GESTION_DE_GATOS.Factura(
				numero,
				fecha,
				total,
				formaDePago,
				estadia,
				clienteError)
		
select distinct M.Factura_Nro,
				M.Factura_Fecha,
				M.Factura_Total+R.costoTotal,
				F.idFormaDePago,
				E.idEstadia,
				C.idCli

from	gd_esquema.Maestra M,
		GESTION_DE_GATOS.Reserva R,
		GESTION_DE_GATOS.Estadia E,
		GESTION_DE_GATOS.ClientesError C,
		GESTION_DE_GATOS.FormaDePago F 

where	M.Factura_Nro is not null and
		M.Reserva_Codigo = R.idReserva and
		E.reserva = R.idReserva and
		C.idCli=R.clienteError and
		F.descripcion = 'Efectivo'
		
		order by Factura_Nro
		
SET IDENTITY_INSERT GESTION_DE_GATOS.Factura OFF
GO

------------------------------------------------------------------------------------------------------------------------------
--Migracion ITEM FACTURAS

--Insertar las estadias
insert into GESTION_DE_GATOS.ItemFactura (factura,monto,cantidad,descripcion)
select m.Factura_Nro,m.Item_Factura_Monto*m.Reserva_Cant_Noches,m.Item_Factura_Cantidad,'Estadia'
	from 
	gd_esquema.Maestra m
	where m.Consumible_Codigo is null and
	m.Factura_Nro is not null
GO

-- Insertar los consumibles agrupados
insert into GESTION_DE_GATOS.ItemFactura (factura,cantidad,monto,descripcion)
select m.Factura_Nro, Sum(m.Item_Factura_Cantidad), sum(m.Item_Factura_Monto), m.Consumible_Descripcion
from gd_esquema.Maestra m
where m.Consumible_Codigo is not null
group by m.Factura_Nro, m.Consumible_Descripcion
order by m.Factura_Nro asc
GO

------------------------------------------------------------------------------------------------------------------------------

-- Migracion de consumibles por estadia

insert into GESTION_DE_GATOS.ConsumibleXEstadia
 (estadia, consumible, habitacion,item)
 
 (
	select 
		es.idEstadia,
		m.Consumible_Codigo,
		hab.idHabitacion,
		i.id
	from 
		gd_esquema.Maestra m,
		GESTION_DE_GATOS.ItemFactura i,
		GESTION_DE_GATOS.Estadia es,
		GESTION_DE_GATOS.ReservaXHabitacion rxh,
		GESTION_DE_GATOS.Habitacion hab
	where m.Consumible_Codigo is not null and
		m.Reserva_Codigo = es.reserva and
		m.Reserva_Codigo = rxh.reserva and
		rxh.habitacion = hab.idHabitacion and
		i.factura = m.Factura_Nro and
		i.descripcion = m.Consumible_Descripcion
		)
order by idEstadia

GO
------------------------------------------------------------------------------------------------------------------------------


