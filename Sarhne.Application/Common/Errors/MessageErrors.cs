using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Errors;

public static class MessageErrors
{
    public static readonly Error MessagesNotAllowed =
        new(
            "Message.MessagesNotAllowed",
            "This user does not accept messages.",
            ErrorType.Validation);

    public static readonly Error AnonymousMessagesNotAllowed =
        new(
            "Message.AnonymousMessagesNotAllowed",
            "This user does not accept anonymous messages.",
            ErrorType.Validation);

    public static readonly Error PhotoMessagesNotAllowed =
        new(
            "Message.PhotoMessagesNotAllowed",
            "This user does not accept photo messages.",
            ErrorType.Validation);

    public static readonly Error CannotMessageYourself =
        new(
            "Message.CannotMessageYourself",
            "You cannot send a message to yourself.",
            ErrorType.Validation);

    public static readonly Error ReceiverNotFound =
        new(
            "Message.ReceiverNotFound",
            "The specified user was not found.",
            ErrorType.NotFound);

    public static readonly Error MessageContentOrPhotoRequired =
        new(
            "Message.ContentOrPhotoRequired",
            "Message must contain content or a photo.",
            ErrorType.Validation);

    public static readonly Error MessageNotFound =
        new(
            "Message.MessageNotFound",
            "Message Not Found.",
            ErrorType.NotFound);

    public static readonly Error CannotLikeOwnMessage =
        new(
            "Message.CannotLikeOwnMessage",
            "You cannot like your own message.",
            ErrorType.Conflict);

}