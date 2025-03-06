using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Modifieritemsmodifiersgroup
{
    public int Id { get; set; }

    public int? ModifierId { get; set; }

    public int? ModifiergroupId { get; set; }

    public virtual Modifieritem? Modifier { get; set; }

    public virtual Modifiersgroup? Modifiergroup { get; set; }
}
