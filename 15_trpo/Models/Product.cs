using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _15_trpo.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public double Stock { get; set; }

    public double Rating { get; set; }

    public string CreatedAt { get; set; } = null!;

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ObservableCollection<Tag> Tags { get; set; } = new ObservableCollection<Tag>();
}
