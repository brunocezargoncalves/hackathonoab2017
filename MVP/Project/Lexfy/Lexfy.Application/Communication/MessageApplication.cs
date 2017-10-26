using Lexfy.Application.Communication.Interfaces;
using Lexfy.Domain.Communication;
using Lexfy.Repository.Communication.Interfaces;
using System;
using System.Collections.Generic;
using Lexfy.Application.Interfaces;

namespace Lexfy.Application.Communication
{
    public class MessageApplication : IMessageApplication
    {
        private readonly IMessageRepository _messageRepository;

        public MessageApplication(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public Message Get(Guid messageId)
        {
            try
            {
                return _messageRepository.Get(messageId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Message> Find(Message message)
        {
            try
            {
                return _messageRepository.Find(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Message message)
        {
            try
            {
                var messageId = Guid.Empty;

                // Message já existe
                // Message já existe, mesmo ???
                if (message != null && message.MessageId != Guid.Empty && Get(message.MessageId) != null)
                {
                    // Marca como atualizado (U) o Message já existente
                    // Sempre existirá apenas um registro com status A de cada Message
                    _messageRepository.Update(new Message()
                    {
                        MessageId = message.MessageId
                    });

                    // Adiciona novo message
                    _messageRepository.Add(new Message()
                    {
                        MessageId = message.MessageId,
                        Subject = message.Subject,
                        MessageClean = message.MessageClean,
                        MessageHtml = message.MessageHtml
                    });

                    messageId = message.MessageId;
                }
                // Message não existe
                else
                {
                    messageId = message != null && message.MessageId != Guid.Empty ? message.MessageId : Guid.NewGuid();

                    // Adiciona novo message
                    _messageRepository.Add(new Message()
                    {
                        MessageId = message.MessageId,
                        Subject = message.Subject,
                        MessageClean = message.MessageClean,
                        MessageHtml = message.MessageHtml
                    });
                }

                return messageId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(Message message)
        {
            try
            {
                _messageRepository.SoftDelete(new Message()
                {
                    MessageId = message.MessageId
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
