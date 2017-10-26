using System;
using System.Collections.Generic;
using Lexfy.Application.Interfaces;
using Lexfy.Domain;
using Lexfy.Repository.Interfaces;

namespace Lexfy.Application
{
    public class TagApplication : ITagApplication
    {
        private readonly ITagRepository _tagRepository;

        public TagApplication(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Tag Get(Guid tagId)
        {
            try
            {
                return _tagRepository.Get(tagId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Tag> Find(Tag tag)
        {
            try
            {
                return _tagRepository.Find(tag);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Tag tag)
        {
            try
            {
                var tagId = Guid.Empty;

                // Tag já existe
                if (tag.TagId != Guid.Empty && Get(tag.TagId) != null)
                {
                    // Atualiza
                    _tagRepository.Update(new Tag
                    {
                        TagId = tag.TagId,
                        Name = tag.Name
                    });

                    tagId = tag.TagId;
                }
                // Tag não existe
                else
                {
                    tagId = Guid.NewGuid();

                    // Adiciona novo Tag
                    _tagRepository.Add(new Tag
                    {
                        TagId = tagId,
                        Name = tag.Name
                    });
                }

                return tagId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(Tag tag)
        {
            throw new NotImplementedException();
        }
    }
}
