using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;

using ClubPool.Web.Controllers;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public class TestViewModel : PagedListViewModelBase<int>
  {
    public TestViewModel(IQueryable<int> source, int page, int pageSize)
      : base(source, page, pageSize)
    {
    }
  }

  public abstract class specification_for_PagedListViewModelBase
  {
    protected static TestViewModel viewModel;
    protected static int page;
    protected static int pageSize;
    protected static int total;

    protected static void init(int p, int ps, int t) {
      page = p;
      pageSize = ps;
      total = t;

      var numbers = new List<int>();
      for(int i = 1; i <= total; i++) {
        numbers.Add(i);
      }
      viewModel = new TestViewModel(numbers.AsQueryable(), page, pageSize);
    }

    protected static void VerifyViewModelState() {
      var first = (page - 1) * pageSize + 1;
      var totalPages = total/pageSize;
      if (total % pageSize > 1) {
        totalPages++;
      }
      int count = pageSize;
      if (page == totalPages && total % pageSize != 0) {
        count = total % pageSize;
      }

      VerifyViewModelState(page, count, first, first + count - 1, total, totalPages);
    }

    protected static void VerifyViewModelState(int page, int count, int first, int last, int total, int totalPages) {
      viewModel.CurrentPage.ShouldEqual(page);
      viewModel.Items.Count().ShouldEqual(count);
      viewModel.Items.First().ShouldEqual(first);
      viewModel.Items.Last().ShouldEqual(last);
      viewModel.Total.ShouldEqual(total);
      viewModel.TotalPages.ShouldEqual(totalPages);
    }
  }

  public class when_asked_for_the_first_page : specification_for_PagedListViewModelBase
  {
    Establish context = () => init(1, 10, 50);

    It should_have_the_correct_state = () => VerifyViewModelState();
  }

  public class when_asked_for_a_middle_page : specification_for_PagedListViewModelBase
  {
    Establish context = () => init(3, 10, 50);

    It should_have_the_correct_state = () => VerifyViewModelState();
  }

  public class when_asked_for_the_last_page : specification_for_PagedListViewModelBase
  {
    Establish context = () => init(5, 10, 50);

    It should_have_the_correct_state = () => VerifyViewModelState();
  }

  public class when_asked_for_the_last_page_with_odd_page_size : specification_for_PagedListViewModelBase
  {
    Establish context = () => init(5, 11, 50);

    It should_have_the_correct_state = () => VerifyViewModelState();
  }

  public class when_asked_for_the_only_page : specification_for_PagedListViewModelBase
  {
    Establish context = () => init(1, 10, 9);

    It should_have_the_correct_state = () => VerifyViewModelState();
  }
}
