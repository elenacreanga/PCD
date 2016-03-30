 // User Submitted Variables
      var project_id = '943812918542';
      var client_id = '943812918542-sbr1m6jspcv3u39er0bb7dcb2rskd0hk.apps.googleusercontent.com';

      var config = {
        'client_id': client_id,
        'scope': 'https://www.googleapis.com/auth/bigquery'
      };
  
      function runQuery(countryName) { 
             
       var query ="SELECT Country, Capital, Population, [area], [languages], Continent FROM [gdelt-bq:extra.countryinfo] WHERE Country ='"+  countryName+ "' ORDER BY Population DESC LIMIT 5;" ;      
       // console.log(query);
       var request = gapi.client.bigquery.jobs.query({
          'projectId': project_id,
          'timeoutMs': '30000',
          'query': query
        });

        request.execute(function(response) {
            var countryinfo = [];  
            var result = response.result.rows[0];
            var country = { 
                      Country           : result.f[0].v, 
                      Capital           : result.f[1].v,  
                      Population        : result.f[2].v, 
                      CountryArea       : result.f[3].v, 
                      CountryLanguages  : result.f[4].v,  
                      Continent         : result.f[5].v, 
                      };
             countryinfo.push('Country : ' + country.Country); 
             countryinfo.push('Capital : ' + country.Capital); 
             countryinfo.push('CountryArea : ' + country.CountryArea); 
             countryinfo.push('Population : ' + country.Population); 
             countryinfo.push('CountryLanguages : ' + country.CountryLanguages);             
            // console.log(country);
              var ul = $('<ul></ul>');
              var elements = countryinfo.length;
                 for(var i=0;i<elements; i++){
                 var div = $('<div></div>').addClass('bar').text(countryinfo[i]);
                 var li = $('<li></li>');
                 li.append(div);          
                 ul.append(li);            
                }

           $('#bigQueryList').append(ul);

           
            countryinfo = [];
          
             // console.log(response.result.rows[0].f[0]);
          //  $('#result_box').html(JSON.stringify(response.result.rows, null));
        });
      }

      function auth() {
        gapi.auth.authorize(config, function() {
            gapi.client.load('bigquery', 'v2');
            $('#client_initiated').html('BigQuery client initiated');
            $('#auth_button').fadeOut();
            $('#projects_button').fadeIn();
            $('#dataset_button').fadeIn();
            $('#query_button').fadeIn();           
        });
      }
