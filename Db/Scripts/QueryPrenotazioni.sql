/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) pr.[Id]
      ,us.Email as UserEmail
	  ,ro.Name as RoleName
      ,sp.Name as SportCentre
	  ,att.Name as Attività
      ,usr.[userId]
      ,[attivitaId]
      ,[sportCentreId]
      ,[Data]
  FROM [aspnet-SportCentre-0c7f4aeb-0d92-4759-a600-f98d56a6ce3d].[dbo].[prenotazioni]  pr
  join attivita att on pr.attivitaId=att.Id 
  join SportCentres sp on pr.sportCentreId=sp.id
  join AspNetUsers us on pr.userId=us.Id 
  join AspNetUserRoles usr on usr.UserId=us.Id 
  join AspNetRoles ro on ro.Id=usr.RoleId
  order by sp.id