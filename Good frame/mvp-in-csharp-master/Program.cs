using System;
using mvp_in_csharp.data;
using mvp_in_csharp.messages;

namespace mvp_in_csharp
{
  class Program
  {
    private  const string SAVING_FILE_PATH = "./messages.json";
    private IMessagesView view;
    static void Main(string[] args)
    {
      Console.WriteLine("=== MESSAGE MANAGER ====");
      Program program = new Program();
      program.SetUp();
      IMessagesView view = program.view;
      view.OnRequestedToShowMenu();
    }

    void SetUp()
    {
      InFileSavingHelper fileSaveHelper = new InFileSavingHelper(new FileParser(), "./messages.json");
      LocalMessageDataSource localDataSource = new LocalMessageDataSource(fileSaveHelper);
      MessageRepository repository = new MessageRepository(localDataSource);
      view = new MessagesView();
      IMessagesPresenter presenter = new MessagesPresenter(repository, view);
    }
  }
}
