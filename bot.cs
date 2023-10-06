using System.Numerics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace project
{

    class Bot
    {
        
        public static Telegram.Bot.Polling.ReceiverOptions? updateHandler = new Telegram.Bot.Polling.ReceiverOptions();

        public static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        async public static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            
            if (update.Message != null)
            {

                if (update.Message.Text != null)
                {
                    Database database = new Database(update);
                    User user = database.ReturnUser();
                    List<User> users = database.ReturnAllUsers();

                    Data data = new Data();


                    Console.WriteLine($"{update.Message.Text} {user.id} {user.username} admin:{user.admin} follow:{user.follow} {update.Message.Date} ");

                    ReplyKeyboardMarkup adminReplyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "/start" },
                        new KeyboardButton[] { "/send" },
                    })
                    {
                        ResizeKeyboard = true,
                    };

                    
                    InlineKeyboardMarkup CourseLink = new InlineKeyboardMarkup(new[]
                    {
                        InlineKeyboardButton.WithUrl(
                            text: Data.CourseButtonText,
                            url: data.CourseUrl)
                    });

                    ReplyKeyboardMarkup userReplyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "/subscribe" },
                        new KeyboardButton[] { "/unsubscribe" },
                        new KeyboardButton[] { "/course" },
                    })
                    {
                        ResizeKeyboard = true,
                    };

                    if (user.admin)
                    {
                        

                        if (update.Message.Text.ToLower().Contains("/start"))
                        {
                            Message startMessage = await botClient.SendTextMessageAsync(
                                chatId: user.id,
                                replyMarkup: adminReplyKeyboardMarkup,
                                text: Data.adminStartText,
                                cancellationToken: token);

                            return;
                        }
                        if (update.Message.Text.ToLower().Contains("/send"))
                        {
                            foreach(User temp in users)
                            {
                                if(temp.follow)
                                {
                                    Message courseMessage = await botClient.SendTextMessageAsync(
                                        chatId: temp.id,
                                        text: data.CourseText,
                                        replyMarkup: CourseLink,
                                        cancellationToken: token);
                                }

                            }

                            return;
                        }
                    }
                    else
                    {

                        if (update.Message.Text.ToLower().Contains("/course"))
                        {
                            Message courseMessage = await botClient.SendTextMessageAsync(
                                chatId: user.id,
                                text: data.CourseText,
                                replyMarkup: CourseLink,
                                cancellationToken: token);

                            return;
                        }

                        if (update.Message.Text.ToLower().Contains("/subscribe"))
                        {
                            if(user.follow == true)
                            {    
                                Message AlradySupscribeMessage = await botClient.SendTextMessageAsync(
                                    chatId: user.id,
                                    text: Data.AlradySubscribeText,
                                    cancellationToken: token);

                                return;
                            }

                            database.ChangeFollow(true);

                            Message supscribeMessage = await botClient.SendTextMessageAsync(
                                chatId: user.id,
                                text: Data.SubscribeText,
                                cancellationToken: token);

                            return;
                        }

                        if (update.Message.Text.ToLower().Contains("/unsubscribe"))
                        {
                            if(user.follow == false)
                            {    
                                Message AlradySupscribeMessage = await botClient.SendTextMessageAsync(
                                    chatId: user.id,
                                    text: Data.AlradyUnsubscribeText,
                                    cancellationToken: token);

                                return;
                            }
                            database.ChangeFollow(false);
                            
                            Message unsupscribeMessage = await botClient.SendTextMessageAsync(
                                chatId: user.id,
                                text: Data.UnsubscribeText,
                                cancellationToken: token);

                            return;
                        }

                        if (update.Message.Text.ToLower().Contains("/start"))
                        {
                            Message startMessage = await botClient.SendTextMessageAsync(
                                chatId: user.id,
                                replyMarkup: userReplyKeyboardMarkup,
                                text: Data.StartText,
                                cancellationToken: token);

                            return;
                        }

                        return;
                    }
                }

                

            }
            return;
        }
    }
}