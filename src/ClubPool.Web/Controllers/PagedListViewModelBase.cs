﻿using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers
{
  public abstract class PagedListViewModelBase<TSource, TResult> : PagedListViewModelBase
  {
    public PagedListViewModelBase() {

    }

    public PagedListViewModelBase(IQueryable<TSource> source, int page, int pageSize, Func<TSource, TResult> converter) {
      Total = source.Count();
      TotalPages = (int)Math.Ceiling((double)Total / (double)pageSize);
      CurrentPage = Math.Min(page, TotalPages);
      var index = Math.Max(CurrentPage - 1, 0);
      Items = source.Page(index, pageSize).ToList().Select(i => converter(i));
      First = index * pageSize + 1;
      Last = First + Items.Count() - 1;
      if (TotalPages > 2) {
        var numPageLinksToShow = 5;
        var first = Math.Max(CurrentPage - numPageLinksToShow / 2, 1);
        var last = Math.Min(CurrentPage + numPageLinksToShow / 2, TotalPages);
        if (last - first < numPageLinksToShow - 1) {
          if (1 == first) {
            last += numPageLinksToShow - (last - first) - 1;
          }
          else {
            first -= numPageLinksToShow - (last - first) - 1;
          }
          last = Math.Min(last, TotalPages);
          first = Math.Max(first, 1);
        }
        FirstPageNumberLink = first;
        LastPageNumberLink = last;
      }
    }

    public IEnumerable<TResult> Items { get; set; }
  }

  public abstract class PagedListViewModelBase
  {
    public int CurrentPage { get; set; }
    public int First { get; set; }
    public int Last { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public int FirstPageNumberLink { get; set; }
    public int LastPageNumberLink { get; set; }
  }
}
