using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Additonal Namespaces
using ChinookSystem.BLL;
using ChinookSystem.Data.POCOs;
using Chinook.UI;
using ChinookSystem.Security;
using Microsoft.AspNet.Identity;
#endregion

public partial class BusinessProcesses_ManagePlayList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            TrackSearchList.DataSource = null;
        }
    }
   

    #region Search Fetch buttons
    protected void ArtistFetch_Click(object sender, EventArgs e)
    {
       
        MessageUserControl.TryRun((ProcessRequest)FetchTracksForArtist);
    }
    public void FetchTracksForArtist()
    {
        int id = int.Parse(ArtistList.SelectedValue);
        TrackController sysmgr = new TrackController();
        List<TracksForPlaylistSelection> results = sysmgr.Get_TracksForPlaylistSelection(id, "Artist");
        TrackSearchList.DataSource = results;
        TrackSearchList.DataBind();
        /*
        this next logic test is to determine if the previous refresh for the
        track list is from the same search method

        if not then the DataPager is reset to page 1 of the displayed data and the
        ListView data is DataBind() is run again (true).

        this code must be placed after the first binding of the data for the DataPager
        to exists.

        http://www.aspsnippets.com/Articles/Selecting-GridView-Row-by-clicking-anywhere-on-the-Row.aspx
        */
        if (!TracksBy.Text.Equals("Artist"))
        {
            int maxrows=(TrackSearchList.FindControl("DataPager1") as DataPager).MaximumRows;
            (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(0,maxrows, true);
        }
        
        //indicates which search method is currently being used.
        TracksBy.Text = "Artist";
    }
    protected void MediaTypeFetch_Click(object sender, EventArgs e)
    {
        MessageUserControl.TryRun((ProcessRequest)FetchTracksForMedia);
    }
    public void FetchTracksForMedia()
    {
        int id = int.Parse(MediaTypeList.SelectedValue);
        TrackController sysmgr = new TrackController();
        List<TracksForPlaylistSelection> results = sysmgr.Get_TracksForPlaylistSelection(id, "Media");
        TrackSearchList.DataSource = results;
        TrackSearchList.DataBind();
        if (!TracksBy.Text.Equals("Media"))
        {
            int maxrows = (TrackSearchList.FindControl("DataPager1") as DataPager).MaximumRows;
            (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(0, maxrows, true);
        }
       
        TracksBy.Text = "Media";
    }

    protected void GenreFetch_Click(object sender, EventArgs e)
    {
        MessageUserControl.TryRun((ProcessRequest)FetchTracksForGenre);
    }
    public void FetchTracksForGenre()
    {
        int id = int.Parse(GenreList.SelectedValue);
        TrackController sysmgr = new TrackController();
        List<TracksForPlaylistSelection> results = sysmgr.Get_TracksForPlaylistSelection(id, "Genre");
        TrackSearchList.DataSource = results;
        TrackSearchList.DataBind();
        if (!TracksBy.Text.Equals("Genre"))
        {
            int maxrows = (TrackSearchList.FindControl("DataPager1") as DataPager).MaximumRows;
            (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(0, maxrows, true);
        }
       
        TracksBy.Text = "Genre";
    }

    protected void AlbumFetch_Click(object sender, EventArgs e)
    {
        MessageUserControl.TryRun((ProcessRequest)FetchTracksForAlbum);
    }
    public void FetchTracksForAlbum()
    {
        int id = int.Parse(AlbumList.SelectedValue);
        TrackController sysmgr = new TrackController();
        List<TracksForPlaylistSelection> results = sysmgr.Get_TracksForPlaylistSelection(id, "Album");
        TrackSearchList.DataSource = results;
        TrackSearchList.DataBind();
        if (!TracksBy.Text.Equals("Album"))
        {
            int maxrows = (TrackSearchList.FindControl("DataPager1") as DataPager).MaximumRows;
            (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(0, maxrows, true);
        }
        
        TracksBy.Text = "Album";
    }
    protected void TrackSearchList_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        MessageUserControl.TryRun(() =>
        {
            switch (TracksBy.Text)
            {
                case "Artist":
                    {
                        (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
                        this.FetchTracksForArtist();
                        break;
                    }
                case "Media":
                    {
                        (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
                        this.FetchTracksForMedia();
                        break;
                    }
                case "Genre":
                    {
                        (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
                        this.FetchTracksForGenre();
                        break;
                    }
                case "Album":
                    {
                        (TrackSearchList.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
                        this.FetchTracksForAlbum();
                        break;
                    }
            }

        });
        
    }
    #endregion

    #region Get a customerid via the log in
    protected int GetUserCustomerId()
    {
        int customerid = 0;
        //is the current user logged on
        if (Request.IsAuthenticated)
        {
            //get the current user name from aspnet User.Identity
            //this name will be the name shown in the right hand corner
            //of the form
            string username = User.Identity.Name;

            //use the security UserManager controller we coded
            //which ties into aspnet.Identity
            //this will be used to get an ApplicationUser instance of the 
            //current user
            //include a using to the controller
            UserManager sysmgr = new UserManager();

            //needs using Mircosoft.Aspnet.Identity
            var applicationuser = sysmgr.FindByName(username);

            //get the customerid from the applicationuser
            customerid = applicationuser.CustomerId == null ? 0 : (int)applicationuser.CustomerId;
        }
        else
        {
            MessageUserControl.ShowInfo("You must log in to manage a playlist.");
        }
        return customerid;
    }
    #endregion

    #region Add a track to a playlist
    protected void TrackSearchList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        int customerid = GetUserCustomerId();

        if (customerid > 0)  //is the user a customer
        {
            ListViewDataItem rowcontents = e.Item as ListViewDataItem;
            string playlistname = PlayListName.Text;
            if (string.IsNullOrEmpty(PlayListName.Text))
            {
                MessageUserControl.ShowInfo("Please enter a playlist name.");
            }
            else
            {
                MessageUserControl.TryRun(() =>
                {
                    //the trackid is attached to each ListView row via the CommandArgument parameter

                    //this method does not make the value visible to the user (or in view source unless
                    //   the hacker decompressed the hidden data)

                    //access to the trackid is done via the ListViewCommandEventsArgs e and is treated
                    //as an object, thus it needs to be cast to a string for the Parse to work

                    PlaylistTrackController sysmgr = new PlaylistTrackController();
                    sysmgr.AddTrackToPlayList(playlistname, int.Parse(e.CommandArgument.ToString()), customerid);
                    List<TracksForPlaylist> results = sysmgr.Get_PlaylistTracks(playlistname, customerid);
                    CurrentPlayList.DataSource = results;
                    CurrentPlayList.DataBind();
                });
            }
        }
        else
        {
            MessageUserControl.ShowInfo("Please use your customer account to manage playlists.");
        }
    }
    #endregion

    #region Fetch a specified playlist for logged customer
    protected void PlayListFetch_Click(object sender, EventArgs e)
    {
        int customerid = GetUserCustomerId();
        if (customerid > 0)
        {
            MessageUserControl.TryRun(() =>
            {
                if (string.IsNullOrEmpty(PlayListName.Text))
                {
                    throw new Exception("Enter a playlist name.");
                }
                else
                {
                    PlaylistTrackController sysmgr = new PlaylistTrackController();
                    List<TracksForPlaylist> results = sysmgr.Get_PlaylistTracks(PlayListName.Text, customerid);
                    CurrentPlayList.DataSource = results;
                    CurrentPlayList.DataBind();
                }
            });
        }
    }

    #endregion
}