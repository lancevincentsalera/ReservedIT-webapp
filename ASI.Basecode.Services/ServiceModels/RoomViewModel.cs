using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RoomViewModel
    {
        [Display(Name = "Room ID")]
        public int RoomId { get; set; }
        [Display(Name = "Room Name")]
        public string RoomName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Room Location")]
        public string Location { get; set; }
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }
        [Display(Name = "Room Equipments")]
        public string Equipments { get; set; }
        public IFormFileCollection RoomGalleryImg { get; set; }

        [Display(Name = "Thumbnail Photo")]
        public IFormFile RoomThumbnailImg { get; set; }
        public string Thumbnail { get; set; }
        public List<RoomGalleryViewModel> _RoomGallery { get; set; }
        public IEnumerable<RoomViewModel> roomList { get; set; }
        public BookingViewModel BookingViewModel { get; set; }
    }
}
