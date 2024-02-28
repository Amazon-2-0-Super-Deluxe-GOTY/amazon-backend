﻿using amazon_backend.Data.Entity;
using System;

public class Review
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; }
    public User _User { get; set; }
    public int ProductId { get; set; }
    public Product _Product { get; set; }
}
