using PB.Book;

namespace PB.Logic
{
  public record ProcessingStartedEvent(IBookInterface book); /**< defines the event and its parameterlist*/
}