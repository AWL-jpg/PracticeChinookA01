use Chinook
go
drop table PlaylistTracks
go
drop table Playlists
go 
create table Playlists
(
	PlaylistId int IDENTITY(1,1) NOT NULL 
	constraint pk_PlayLists_PlaylistId primary key,
	Name nvarchar(50) NOT NULL,
	CustomerId int NULL
	constraint fk_PlayListCustomers_Customer references Customers(CustomerId)
)
go
CREATE TABLE PlaylistTracks
(
	PlaylistId int NOT NULL
	constraint fk_PlayListTracksPlayLists_PlaylistId references Playlists(PlaylistId),
	TrackId int NOT NULL
	constraint fk_PlayListTracksTracks_TrackId references Tracks(TrackId),
	TrackNumber int NOT NULL,
	CONSTRAINT PK_PlaylistTracks_PlaylistIdTrackID PRIMARY KEY CLUSTERED (PlaylistId,	TrackId ASC)
)
go