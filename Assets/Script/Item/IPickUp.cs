using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPS.item
{
      interface IPickUp
      {
            BaseItem Item { get; set; }
            void PickUp();
      }
}

