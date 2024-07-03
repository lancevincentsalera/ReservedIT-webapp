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
        public string RoomDescription { get; set; }
        [Display(Name = "Room Location")]
        public string RoomLocation { get; set; }
        [Display(Name = "Capacity")]
        public int RoomCapacity { get; set; }
        [Display(Name = "Available Equipments")]
        public string RoomFacility { get; set; }
        [Display(Name = "Room Photos")]
        public IFormFileCollection RoomGalleryImg { get; set; }
        public string RoomImage { get; set; }
        [Display(Name = "Thumbnail Photo")]
        public IFormFile RoomThumbnailImg { get; set; }
        public string RoomThumbnail { get; set; }
        public List<RoomGalleryViewModel> RoomGallery { get; set; }
    }
    public class RoomViewModelList
    {
        [Display(Name = "RoomName", ResourceType = typeof(Resources.Views.Screen))]
        public string RoomNameFilter { get; set; }
        public IEnumerable<RoomViewModel> roomList { get; set; }
    }
}
