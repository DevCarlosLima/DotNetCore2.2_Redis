using System;
using System.Collections.Generic;
using Books.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Books.Services {
    public class BookService {
        private readonly IDistributedCache _dc;
        private readonly RedisOptions _ro;

        public BookService (IDistributedCache dc, RedisOptions ro) {
            _dc = dc;
            _ro = ro;
        }

        public IEnumerable<BookModel> GetAll () {
            var books = _ro.GetAll<BookModel>(_dc);
            return books;
        }

        public BookModel GetByKey (string key) {
            var result = _dc.GetString (key);

            if (string.IsNullOrEmpty (result))
                return null;

            return JsonConvert.DeserializeObject<BookModel> (result);
        }

        public string UpSert (BookModel book) {
            var exist = GetByKey (book.Id.ToString ());

            if (exist == null) {
                var id = Guid.NewGuid ();
                
                book.Id = id;
                _dc.SetString (
                    id.ToString (),
                    JsonConvert.SerializeObject (book).ToString (),
                    _ro.CacheOptions
                );

                return id.ToString();
            } else {
                _dc.SetString (
                    book.Id.ToString (),
                    JsonConvert.SerializeObject (book).ToString (),
                    _ro.CacheOptions
                );

                return book.Id.ToString();
            }
        }

        public void Remove (string key) {
            _dc.Remove (key);
        }
    }
}